---
title: 状态指示灯
summary: 显示设备反馈状态的只读指示灯，提供本地化文字和可选闪烁。
source-hash: b9d02a496c7b
---

<Demo name="States" />

## 何时使用

使用 `StatusIndicator` 在上位机中显示 PLC、网关、水泵或电机的反馈状态。六种明确状态为未知、离线、待机、运行、警告和故障。默认显示状态文字，颜色不是唯一提示。初始状态为未知。

## 何时不使用

操作请求使用 `Switch`，简短元数据使用 `Badge`。测量值使用 `Thermometer`、`Gauge` 或图表。指示灯不会发送命令、连接设备、查询数据库、检测超时或确认报警。

## 用法

```xml
<arc:StatusIndicator State="{Binding PumpState}"
                     AutomationProperties.Name="水泵 P-101" />
```

应用将收到的反馈映射为 `StatusIndicatorState`，并在 UI 线程更新绑定属性。操作请求与已确认反馈分别保存。根据应用策略，将缺测或过期数据映射为 Unknown，将确认的断开连接映射为 Offline。报警阈值、通信协议与重连逻辑由应用负责。

`Label` 可以覆盖状态文字；null 使用当前语言的状态名称。`ShowLabel` 默认为 true；隐藏时在附近提供等价的可见状态文字。`DisplayText` 支持只读绑定。`IsBlinking` 默认为 false；应用可按需明确开启，突出需要关注的状态。演示展示六种常亮状态，以及通过按钮和已有 Switch 控件调节的一盏反馈灯。

## API 参考

<!-- api: StatusIndicator -->

## 无障碍

控件提供当前状态的只读文本值，即使 `ShowLabel` 为 false 也保留。它不接受焦点，也不是切换按钮。默认无障碍名称为本地化的“设备状态”；通过 `AutomationProperties.Name` 标识具体设备。状态文字变化采用礼貌通知，闪烁不会改变无障碍值。

## 动效

状态文字和颜色立即更新。可选闪烁只作用于灯，周期为一秒，在 ControlTheme 的 Full 动效选择器中声明。Reduced 和 Off 保持常亮。关闭闪烁或卸载控件会移除动画选择器。没有每控件定时器，也不会延迟真实告警。

## 响应式行为

`StatusIndicatorToken.Diameter` 默认 16 个设备无关像素，`LabelGap` 默认 8。布局中的灯宽高使用同一直径，文字采用自然尺寸。主题覆盖动态生效；颜色、圆角、边框、字体、字号和中等字重来自 ArcDesign 资源。为翻译后的状态文字留出空间；仅显示灯时，在其他可见位置提供状态文字。

## 相关组件

命令输入使用 `Switch`，元数据使用 `Badge`，温度使用 `Thermometer`，趋势使用 `RealtimeStream`。在设备面板中组合这些控件，由应用提供数据。
