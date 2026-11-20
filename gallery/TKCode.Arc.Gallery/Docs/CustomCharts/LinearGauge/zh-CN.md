---
title: 线性仪表
summary: 以竖向柱条展示拉力、压力、转速等连续量程读数。
source-hash: e25d02cc4c00
---

<Demo name="Readings" />

## 何时使用

使用 `LinearGauge` 展示连续数值在量程中的位置。窄柱条表达比例，实际数字、单位和明确的阈值状态保持可见。演示并排展示拉力、压力和转速。

## 何时不使用

储罐或容器使用 `LevelIndicator`，圆形仪表使用 `Gauge`，历史趋势使用 `LineChart`。设置目标值使用滑块。本控件只读，不采集数据或控制设备。

## 用法

```xml
<arc:LinearGauge Label="拉力" Unit="N" Value="450"
                 Minimum="0" Maximum="1000"
                 LowThreshold="200" HighThreshold="800" Decimals="0" />
```

数值和阈值使用同一单位。`Unit` 只显示文字，不转换数值。低于 `LowThreshold` 为 `Low`，达到或超过 `HighThreshold` 为 `High`，中间区间为 `Normal`。`Decimals` 控制实际数字的小数位（0～6，默认 1）。默认量程 0～100，阈值 20/80。

将 `Value` 和 `Quality` 绑定到应用数据，设备更新应切换到 UI 线程。有限越界读数仍显示实际数字，柱条停在边界。上下限相等或倒置、阈值顺序倒置时显示 `InvalidRange` 并清空轨道。拒绝 NaN 和无穷值。

`Quality` 默认 `Good`。`NoData`、`Stale`、`Error` 隐藏柱条并用破折号替代数字；质量文字优先于阈值和量程错误文字。保存的 `Value` 和 `State` 不变。使用保存读数前先检查质量。时间戳、超时和传感器诊断由应用负责。先更新数值，再恢复 `Good`。

`LowTone`、`NormalTone`、`HighTone` 默认 Warning、Accent、Danger。轨道左侧短标记表示量程内的阈值。`LowLabel`、`NormalLabel`、`HighLabel`、`NoDataLabel`、`StaleLabel`、`ErrorLabel` 可覆盖文字；null 跟随应用语言。`ShowScale` 控制刻度显示，`TickCount` 设置 1～10 个分段。

演示的滑块和按钮只改变拉力。质量按钮更新绑定的数据源；读数按钮即使重复相同数值也恢复 Good。压力和转速是独立示例。

## API 参考

<!-- api: LinearGauge -->

## 无障碍

控件提供只读数值范围和只读文本，以 `Label` 为无障碍名称。帮助文字描述实际读数和状态；非 Good 时只报告质量。数值接口保留保存的、限制到量程内的数值，使用方应先检查质量再将其视为当前值。控件没有交互操作。

## 动效

只有连续 Good 读数的柱条高度使用可中断的 Arc 弹簧。数字、状态和颜色立即更新。Reduced、Off、首个有效样本、质量恢复及重新挂载均直接落到目标。颜色、圆角、字体和字重跟随动态 ArcDesign 资源。

## 响应式行为

参考坐标为 220 × 360。`LinearGaugeToken.DialWidth` 和 `DialHeight` 限制整体等比缩放，更小容器进一步缩小。两者设为 440 × 720 可显示两倍尺寸，440 × 360 仍显示 220 × 360。`TrackWidth`（默认 36，限制 16～64）和 `OutlineThickness`（默认 1.5，限制 0～4）使用参考坐标单位。轨道圆角跟随 `RadiusControl`。窄轨道旁可完整显示四位刻度数字；极大或极小刻度使用科学记数法。

## 相关

容器充满程度使用 `LevelIndicator`，温度使用 `Thermometer`，圆形量程仪表使用 `Gauge`，连续样本使用 `RealtimeStream`。
