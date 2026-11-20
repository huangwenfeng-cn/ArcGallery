---
title: 液位计
summary: 只读液位仪表，包含罐体、液面、刻度、实际读数和明确的液位状态。
source-hash: 44f318d6dda4
---

<Demo name="Readings" />

<Demo name="QualityStates" />

## 何时使用

用 `LevelIndicator` 显示储罐、料仓、水池或料斗的已装量。数字保留真实读数，液体表示它在量程中的位置；状态文字帮助用户判断液位过高或过低，无需仅依赖颜色。

## 何时不用

历史和趋势使用 `LineChart` 或 `RealtimeStream`。设定补液目标使用输入框或滑块；液位计只负责显示读数。报警、泵阀控制和液位联锁逻辑由应用负责，不能依据动画中的液面高度执行控制。

## 用法

`Value`、`Minimum`、`Maximum`、`LowThreshold` 和 `HighThreshold` 必须使用相同的测量单位。读数小于低阈值为 `Low`，大于等于高阈值为 `High`，中间为 `Normal`。单位文字不会自动转换数值。

```xml
<arc:LevelIndicator Label="储罐液位" Value="45"
                    Minimum="0" Maximum="100"
                    LowThreshold="20" HighThreshold="80" />
```

将 `Value` 绑定到视图模型中的测量值，并在 UI 线程更新。越界读数仍完整显示，液面停在量程边界。量程颠倒、上下限相等或阈值颠倒时，显示 `InvalidRange` 并清空罐体；不接受 NaN 或无穷值。

将 `Quality` 绑定到应用提供的 `MeasurementQuality`：`Good`（默认）、`NoData`、`Stale` 或 `Error`。非 Good 时隐藏液体，当前数字替换为破折号；质量文字优先于液位及量程错误文字。`Value` 和阈值 `State` 仍保留，使用保存值前应检查质量。时间戳、超时规则和传感器诊断由应用负责，控件不轮询。恢复时先更新数值，再设置 `Good`。

`NoDataLabel`、`StaleLabel`、`ErrorLabel` 覆盖质量文字；null 时跟随应用语言。交互示例将 `Quality` 绑定到可观察的采样数据源，增加质量与恢复按钮。移动滑块或点击液位按钮恢复 `Good`，同值按钮也能恢复。

示例只保留一个液位计，滑块和液位按钮更新同一个读数。液体在罐体的上下圆弧之间填充；液面与刻度共用相同的内部高度。

`LowTone`、`NormalTone` 和 `HighTone` 选择已有的 Arc 色调。三种状态的文字可通过 `LowLabel`、`NormalLabel`、`HighLabel` 覆盖，为 null 时跟随应用语言。`ShowScale` 控制刻度显示，`TickCount` 控制分格数。

## API 参考

<!-- api: LevelIndicator -->

## 无障碍

液位计提供只读的范围值，以标签作为无障碍名称，帮助文本包含实际读数、单位与状态。控件不接收焦点或用户输入。`State` 立即变化，与液体动画无关。

非 Good 时，帮助文字和只读文本值只报告质量状态。为保持兼容，原数值范围接口仍返回保存的、限制到量程内的数值；使用者应检查质量后再视为当前值。

## 动效

仅液体使用可中断的 Arc 弹簧。数字、颜色和状态始终描述实际读数。Reduced 与 Off 模式立即到达目标。切换主题时颜色和字体自动跟随 ArcDesign 令牌；标题和读数使用 `WeightMedium`，状态和刻度使用 `WeightRegular`。

质量变化立即生效并停止液体动效。质量恢复或重新挂载后的首个读数直接到达目标。

## 响应式行为

绘图参考坐标为 220 × 360。`LevelIndicatorToken.DialWidth` 与 `DialHeight` 共同约束最大显示尺寸，测量和绘制采用相同的等比例缩放规则，因此宽高令牌比例不同也不会拉伸罐体。两者设为 440 × 720 时允许显示两倍大小的仪表；设为 440 × 360 时仍适配为 220 × 360。较小容器会进一步缩小显示。其余组件令牌以绘图参考坐标单位控制罐宽和描边厚度；罐体圆角跟随 ArcDesign 的 `RadiusControl` 令牌。

## 相关组件

通用量程仪表使用 `Gauge`，连续接收液位样本使用 `RealtimeStream`，历史液位使用 `LineChart`。
