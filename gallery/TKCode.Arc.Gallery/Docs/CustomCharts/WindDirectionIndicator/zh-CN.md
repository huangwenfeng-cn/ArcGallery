---
title: 风向指示器
summary: 实时风向罗盘，提供可选方向读数以及明确的静风、无数据状态。
source-hash: c80fc45851ea
---

<Demo name="Readings" />

<Demo name="QualityStates" />

## 何时使用

用 `WindDirectionIndicator` 显示 HMI 或气象面板中的当前气象风向。实心指针指向风的来向：北为 0°、东为 90°、南为 180°、西为 270°。双圆环之间形成刻度带，外侧完整显示 16 个角度标签，间隔 22.5°；内侧显示八方位，四主方位更大，配有四个三角定位标记。可选读数显示最近的十六方位名称。

## 何时不用

历史风向和风速分布使用 `WindRoseChart`，风速趋势使用 `LineChart` 或 `RealtimeStream`。罗盘负责显示测量值，不选择方向或执行设备控制。

## 用法

```xml
<arc:WindDirectionIndicator Direction="22.5" ShowReading="True" />
```

将 `Direction` 绑定到视图模型，并在 UI 线程更新。有限角度归一化到 [0, 360)：360 表示北，-90 表示西。`ShowReading` 默认 false；开启后在罗盘下方显示角度与方位名称。可见标题和风速放在外层面板，风速可使用独立 `MetricCard` 等读数组件。

应用判定静风时设置 `IsCalm`：隐藏指针，在圆心显示静风文字。采样不可用时设置 `HasData=false`：无数据状态优先于静风，隐藏指针，在圆心显示无数据文字。即使关闭 `ShowReading`，状态文字仍可见，状态切换不改变罗盘尺寸。收到有效采样后恢复状态标记。静风阈值和采样质量由应用处理。

将 `Quality` 绑定到应用提供的 `MeasurementQuality`：`Good`（默认）、`NoData`、`Stale` 或 `Error`。非 Good 时以圆心状态文字替代指针和方向读数，即使 `ShowReading=false` 也可见；保存的 `Direction` 不变。`HasData=false` 优先显示无数据，否则质量状态优先于静风。时间戳、超时规则和采集诊断由应用负责。收到有效采样后更新 `Direction` 并恢复质量和状态标记。

`StaleLabel`、`ErrorLabel` 覆盖质量文字；null 时跟随应用语言。交互示例将 `Quality` 绑定到可观察的采样数据源，增加过期、异常与恢复按钮。移动滑块恢复 `Good`。

`Label` 覆盖无障碍名称，`CalmLabel`、`NoDataLabel` 覆盖状态文字；null 时跟随应用语言。方位名称支持英文、简体中文和繁体中文。示例包含一个罗盘、方向滑块、两个跨北向按钮、静风和无数据按钮，以及方向读数开关。移动滑块会恢复测量读数。

## API 参考

<!-- api: WindDirectionIndicator -->

## 无障碍

罗盘提供只读文本值，包括实际角度和方位名称，或明确的静风、无数据状态；隐藏可视读数不影响无障碍读数。本地化组件名称作为默认无障碍名称，`Label` 可覆盖，应用显式设置的自动化名称优先。控件不接收焦点或用户输入。

采样过期或异常时，文本值与帮助文字报告质量状态，不包含保存的旧方向。

## 动效

只有箭头使用可中断的 Arc 弹簧，从当前视觉位置沿最短路径旋转：359° 到 1° 只跨越北向 2°。文字立即更新。Reduced 和 Off 模式直接到达目标。静风、无数据或重新挂载后的首个有效方向立即显示。

质量变化停止指针动效；从任何非 Good 状态恢复后的首个有效方向立即显示。

## 响应式行为

默认绘图参考坐标为 320 × 320，开启 `ShowReading` 后为 320 × 352。`WindDirectionIndicatorToken.DialWidth` 和 `DialHeight` 默认 320、352，共同约束最大等比例显示尺寸，测量、布局和绘制共用缩放规则，因此宽高覆盖比例不同也保持圆盘为圆形。`NeedleLength`（68）、`NeedleWidth`（48）、`RingWidth`（18）、`OutlineThickness`（1.5）使用绘图单位。指针长度受限以避开方位文字，宽度和刻度带间距也有范围限制。颜色、字体及 `WeightMedium`、`WeightRegular` 跟随动态 ArcDesign 资源。

## 相关组件

温度使用 `Thermometer`，湿度、能见度、气压和降雨量读数使用 `MetricCard`，风向统计使用 `WindRoseChart`。
