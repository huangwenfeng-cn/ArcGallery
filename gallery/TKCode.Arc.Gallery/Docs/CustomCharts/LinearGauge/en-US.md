---
title: Linear gauge
summary: A vertical range meter for force, pressure, speed and other continuous readings.
---

<Demo name="Readings" />

## When to use

Use `LinearGauge` to show a continuous reading within a range. A narrow bar expresses its position; the actual number, unit and named threshold state remain visible. The demo compares force, pressure and rotational speed.

## When not to use

Use `LevelIndicator` for a tank or vessel, `Gauge` for a circular meter, and `LineChart` for history. Use a slider for setting a target. This read-only meter does not collect data or control equipment.

## Usage

```xml
<arc:LinearGauge Label="拉力" Unit="N" Value="450"
                 Minimum="0" Maximum="1000"
                 LowThreshold="200" HighThreshold="800" Decimals="0" />
```

All values and thresholds share the same unit. `Unit` is display text and does not convert values. Below `LowThreshold` the state is `Low`; at or above `HighThreshold` it is `High`; the remaining interval is `Normal`. `Decimals` controls the actual reading's decimal places (0–6, default 1). The default range is 0–100 with thresholds 20/80.

Bind `Value` and `Quality` to application data and marshal updates to the UI thread. Finite out-of-range values remain printed while the bar stops at the boundary. Equal or inverted bounds, or inverted thresholds, yield `InvalidRange` and an empty track. Non-finite values are rejected.

`Quality` defaults to `Good`. `NoData`, `Stale` and `Error` hide the bar and replace the number with a dash; quality text takes priority over threshold and invalid-range text. The saved `Value` and `State` remain unchanged. Check quality before using a saved reading. The application owns timestamps, timeout rules and sensor diagnostics. Update the value before restoring `Good`.

`LowTone`, `NormalTone` and `HighTone` default to Warning, Accent and Danger. The short marks to the left of the track show in-range thresholds. `LowLabel`, `NormalLabel`, `HighLabel`, `NoDataLabel`, `StaleLabel` and `ErrorLabel` override text; null follows the application language. `ShowScale` controls the scale, and `TickCount` sets 1–10 divisions.

The demo slider and buttons change only the force reading. Quality buttons update a bound sample source. A reading button restores Good even when repeating the same value. Pressure and speed provide independent examples.

## API reference

<!-- api: LinearGauge -->

## Accessibility

The meter exposes a read-only range and read-only text, with `Label` as its accessible name. Help text describes the actual reading and state; non-Good quality reports only the quality label. The numeric provider retains the saved, clamped value, so consumers must check quality before treating it as current. The control is not interactive.

## Motion

Only bar height uses an interruptible Arc spring for consecutive Good readings. The number, state and color update immediately. Reduced and Off motion, the first valid sample, quality recovery and reattachment land directly on the target. Colors, corner radius, fonts and font weights follow dynamic ArcDesign resources.

## Responsive behavior

The design box is 220 × 360. `LinearGaugeToken.DialWidth` and `DialHeight` bound uniform scaling; a smaller container reduces it further. Setting both to 440 × 720 permits twice the size, while 440 × 360 still displays 220 × 360. `TrackWidth` (default 36, clamped to 16–64) and `OutlineThickness` (default 1.5, clamped to 0–4) use design-box units. The track radius follows `RadiusControl`. Four-digit scale labels fit beside the narrow track; very large or small scale values use scientific notation.

## Related

Use `LevelIndicator` for vessel fullness, `Thermometer` for temperature, `Gauge` for a circular range meter, and `RealtimeStream` for incoming samples.
