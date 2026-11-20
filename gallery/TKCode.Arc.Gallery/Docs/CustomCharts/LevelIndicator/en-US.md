---
title: Level indicator
summary: A read-only level meter that fills a vessel with a liquid column beside a scale, the actual reading and named threshold states.
---

<Demo name="Readings" />

<Demo name="QualityStates" />

## When to use

Use `LevelIndicator` to show how full a tank, silo, reservoir or hopper is. It keeps the actual reading visible while the liquid shows its position within the configured range. A named state makes low and high levels understandable without relying on color.

## When not to use

Use `LineChart` or `RealtimeStream` for history and trends. Use an input or slider when the person must set a fill target; this meter only displays a reading. Alarms, pump or valve control and level interlock logic belong to the application, not the animated liquid.

## Usage

`Value`, `Minimum`, `Maximum`, `LowThreshold` and `HighThreshold` use the same measurement unit. Below `LowThreshold` the state is `Low`; at or above `HighThreshold` it is `High`; values in between are `Normal`. The unit text does not perform a conversion.

```xml
<arc:LevelIndicator Label="储罐液位" Value="45"
                    Minimum="0" Maximum="100"
                    LowThreshold="20" HighThreshold="80" />
```

Bind `Value` to the view model's measurement and marshal device updates to the UI thread. An out-of-range value is still printed, while the liquid stops at the range boundary. An inverted or equal range, or inverted thresholds, shows `InvalidRange` with an empty vessel. Non-finite values are rejected.

Bind `Quality` to application-provided `MeasurementQuality`: `Good` (default), `NoData`, `Stale` or `Error`. Non-Good quality hides the liquid and replaces the current number with a dash; its text takes priority over level text, including invalid-range text. `Value` and the threshold `State` are preserved, so check quality before using the saved value. The application owns timestamps, timeout rules and sensor diagnostics; the control has no polling timer. Update the value before restoring `Good`.

`NoDataLabel`, `StaleLabel` and `ErrorLabel` override quality text; null follows the application's language. The interactive demo binds `Quality` to its observable sample source and includes quality and recovery buttons. Moving the slider or pressing a level button restores `Good`, even when a button repeats the same value.

The demo shows one level indicator; its slider and level buttons update that reading. The liquid fills the vessel between its rounded top and bottom. The liquid level and scale share the same interior height.

`LowTone`, `NormalTone` and `HighTone` select the existing Arc color families. `LowLabel`, `NormalLabel` and `HighLabel` override the built-in labels; null uses the application's language. `ShowScale` hides the scale, and `TickCount` controls its divisions.

## API reference

<!-- api: LevelIndicator -->

## Accessibility

The meter exposes a read-only range value with its label as the accessible name. Help text includes the actual reading, unit and state. It is not focusable or interactive. `State` updates immediately, independently of the animated liquid.

For non-Good quality, help text and the read-only text value report only the quality label. The existing numeric range provider retains the saved, clamped value for compatibility; consumers must check quality before treating it as current.

## Motion

Only the liquid column uses an interruptible Arc spring. The number, color and state always describe the actual reading. Reduced and Off motion land on the target immediately. Theme changes follow ArcDesign colors and fonts automatically, including `WeightMedium` for the label and reading and `WeightRegular` for the state and scale.

Quality changes are immediate and stop liquid motion. The first reading after quality recovery or reattachment lands directly on the target.

## Responsive behavior

The drawing reference is 220 × 360. `LevelIndicatorToken.DialWidth` and `DialHeight` bound the maximum display size; measurement and drawing use the same uniform scale, so different width/height overrides never stretch the vessel. Setting both to 440 × 720 permits a twice-sized meter; 440 × 360 still fits a 220 × 360 meter. A smaller container further reduces the scale. The other component tokens control vessel width and outline thickness in drawing-reference units; the vessel corner radius follows the ArcDesign `RadiusControl` token.

## Related

Use `Gauge` for a general range meter, `RealtimeStream` for incoming level samples, and `LineChart` for historical readings.
