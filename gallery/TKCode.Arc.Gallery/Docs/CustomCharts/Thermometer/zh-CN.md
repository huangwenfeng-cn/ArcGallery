---
title: 温度计
summary: 只读温度仪表，包含液柱、刻度、实际读数和明确的高低温状态。
source-hash: dc222a6e8299
---

<Demo name="Readings" />

<Demo name="QualityStates" />

## 何时使用

用 `Thermometer` 显示设备、房间、冷库或加热器的当前温度。数字保留真实读数，液柱表示它在量程中的位置；状态文字帮助用户判断高低温，无需仅依赖颜色。

## 何时不用

历史和趋势使用 `LineChart` 或 `RealtimeStream`。设定温度使用输入框或滑块；温度计只负责显示读数。报警与控制逻辑由应用负责，不能依据动画中的液柱高度执行控制。

## 用法

`Value`、`Minimum`、`Maximum`、`LowThreshold` 和 `HighThreshold` 必须使用相同的温度单位。读数小于低温阈值为 `Low`，大于等于高温阈值为 `High`，中间为 `Normal`。单位文字不会自动转换温度。

```xml
<arc:Thermometer Label="设备温度" Value="36.5"
                 Minimum="-20" Maximum="100"
                 LowThreshold="10" HighThreshold="70" />
```

将 `Value` 绑定到视图模型中的测量值，并在 UI 线程更新。越界读数仍完整显示，液柱停在量程边界。量程颠倒、上下限相等或阈值颠倒时，显示 `InvalidRange` 并清空液柱；不接受 NaN 或无穷值。

将 `Quality` 绑定到应用提供的 `MeasurementQuality`：`Good`（默认）、`NoData`、`Stale` 或 `Error`。非 Good 时隐藏液柱，当前数字替换为破折号；质量文字优先于高低温及量程错误文字。`Value` 和阈值 `State` 仍保留，使用保存值前应检查质量。时间戳、超时规则和传感器诊断由应用负责，控件不轮询。恢复时先更新数值，再设置 `Good`。

`NoDataLabel`、`StaleLabel`、`ErrorLabel` 覆盖质量文字；null 时跟随应用语言。交互示例将 `Quality` 绑定到可观察的采样数据源，增加质量与恢复按钮。移动滑块或点击温度按钮恢复 `Good`，同值按钮也能恢复。

`LowTone`、`NormalTone` 和 `HighTone` 选择已有的 Arc 色调。三种状态的文字可通过 `LowLabel`、`NormalLabel`、`HighLabel` 覆盖，为 null 时跟随应用语言。`ShowScale` 控制刻度显示，`TickCount` 控制分格数。

示例只保留一个温度计，滑块和温度按钮更新同一个读数。达到量程上限时，液体会完整填充顶部圆弧；液面与刻度共用相同的内部高度。

## API 参考

<!-- api: Thermometer -->

## 无障碍

温度计提供只读的范围值，以标签作为无障碍名称，帮助文本包含实际读数、单位与状态。控件不接收焦点或用户输入。`State` 立即变化，与液柱动画无关。

非 Good 时，帮助文字和只读文本值只报告质量状态。为保持兼容，原数值范围接口仍返回保存的、限制到量程内的数值；使用者应检查质量后再视为当前值。

## 动效

仅液柱使用可中断的 Arc 弹簧。数字、颜色和状态始终描述实际读数。Reduced 与 Off 模式立即到达目标。切换主题时颜色和字体自动跟随 ArcDesign 令牌；标题和读数使用 `WeightMedium`，状态和刻度使用 `WeightRegular`。

质量变化立即生效并停止液柱动效。质量恢复或重新挂载后的首个读数直接到达目标。

## 响应式行为

绘图参考坐标为 220 × 360。`ThermometerToken.DialWidth` 与 `DialHeight` 共同约束最大显示尺寸，测量和绘制采用相同的等比例缩放规则，因此宽高令牌比例不同也不会拉伸球泡。两者设为 440 × 720 时允许显示两倍大小的仪表；设为 440 × 360 时仍适配为 220 × 360。较小容器会进一步缩小显示。其余组件令牌以绘图参考坐标单位控制管宽、球泡半径和描边厚度。

## 相关组件

通用量程仪表使用 `Gauge`，连续接收温度样本使用 `RealtimeStream`，历史温度使用 `LineChart`。
