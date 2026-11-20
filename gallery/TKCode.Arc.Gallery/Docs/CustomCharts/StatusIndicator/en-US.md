---
title: Status indicator
summary: A read-only device status lamp with localized text and optional blinking.
---

<Demo name="States" />

## When to use

Use `StatusIndicator` to show PLC, gateway, pump or motor feedback in an HMI. The six explicit states are Unknown, Offline, Idle, Running, Warning and Fault. State names remain visible by default, so color is not the only signal. Unknown is the safe initial state.

## When not to use

Use `Switch` for an operation request and `Badge` for compact metadata. Use `Thermometer`, `Gauge` or charts for measured quantities. The indicator does not send commands, connect to devices, query a database, detect a timeout or acknowledge alarms.

## Usage

```xml
<arc:StatusIndicator State="{Binding PumpState}"
                     AutomationProperties.Name="Pump P-101" />
```

The application maps received feedback to `StatusIndicatorState` and updates the bound property on the UI thread. Keep requested operation and confirmed feedback separate. Map missing or stale samples to Unknown and a confirmed disconnection to Offline according to your application policy. Alarm thresholds, communication protocols and reconnection belong in the application.

`Label` overrides the status text; null uses the current language's state name. `ShowLabel` defaults to true. If you hide it, place equivalent visible state text nearby. `DisplayText` is available for read-only binding. `IsBlinking` defaults to false; enable it explicitly for states your application wants to draw attention to. The demo shows all six steady states and one feedback lamp controlled by buttons and existing Switch controls.

## API reference

<!-- api: StatusIndicator -->

## Accessibility

The control exposes a read-only text value with the current status, even when `ShowLabel` is false. It is not focusable or a toggle. The default accessible name is localized “Device status”; set `AutomationProperties.Name` to identify the device. Text changes are announced politely; pulses never change the accessible value.

## Motion

State text and color update immediately. Optional blinking is a one-second opacity pulse on the lamp only, expressed in the ControlTheme under Full motion. Reduced and Off remain steady. Disabling blinking or detaching removes the animation selector. There is no per-control timer or delayed alarm state.

## Responsive behavior

`StatusIndicatorToken.Diameter` defaults to 16 device-independent pixels and `LabelGap` to 8. The same diameter sets the lamp's width and height in layout; the text uses its natural size. Theme overrides apply dynamically. Colors, pill radius, border, font family, font size and medium weight use ArcDesign resources. Allow enough room for translated status text; for a lamp-only layout, provide visible state text elsewhere.

## Related

Use `Switch` for commands, `Badge` for metadata, `Thermometer` for temperature and `RealtimeStream` for trends. Compose these controls in a device panel using application-owned data.
