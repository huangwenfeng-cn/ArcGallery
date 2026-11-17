---
title: Wind direction indicator
summary: A live wind-origin compass with optional direction reading and explicit calm or missing-data states.
---

<Demo name="Readings" />

## When to use

Use `WindDirectionIndicator` for the current meteorological wind direction in an HMI or weather panel. The solid pointer points toward the origin of the wind: north is 0°, east 90°, south 180° and west 270°. Two rings frame sixteen ticks and external angle labels at 22.5° intervals. Eight direction labels sit inside, with larger cardinal labels and four triangular markers. The optional reading names the nearest of sixteen directions.

## When not to use

Use `WindRoseChart` for historical direction and speed distributions. Use `LineChart` or `RealtimeStream` for a speed trend. This compass displays measurements; it does not select a bearing or execute device control.

## Usage

```xml
<arc:WindDirectionIndicator Direction="22.5" ShowReading="True" />
```

Bind `Direction` to the view model and update it on the UI thread. Finite direction values wrap into [0, 360): 360 means north, and -90 means west. `ShowReading` defaults to false; enable it to show the bearing and named direction below the compass. Place a visible heading and wind speed in the surrounding panel, using a separate readout such as `MetricCard`.

Set `IsCalm` when the application reports calm wind: the pointer disappears and a calm label appears in the center. Set `HasData` to false when the sample is unavailable: this overrides calm, hides the pointer, and displays the missing-data label in the center. These states remain visible with `ShowReading` disabled and do not resize the compass. Restore the flags when a valid sample arrives. The application owns calm thresholds and sample quality.

`Label` overrides the accessible name; `CalmLabel` and `NoDataLabel` override state text. Null follows the application language. Compass directions are localized in English, Simplified Chinese and Traditional Chinese. The demo contains one compass, a direction slider, two north-crossing buttons, calm/missing-data buttons and a reading toggle. Moving the slider restores a measured reading.

## API reference

<!-- api: WindDirectionIndicator -->

## Accessibility

The compass provides a read-only text value with the actual bearing and named direction, or the explicit calm/missing-data state, even when the visual reading is hidden. The localized component name is the default accessible name; `Label` can override it and an application-provided automation name takes precedence. It is not focusable or interactive.

## Motion

Only the arrow animates with an interruptible Arc spring. It takes the shortest path from its current visual position: 359° to 1° crosses north by 2°. Text updates immediately. Reduced and Off motion jump to the target. The first sample after calm, missing data or reattachment appears immediately.

## Responsive behavior

The drawing reference is 320 × 320, or 320 × 352 with `ShowReading` enabled. `WindDirectionIndicatorToken.DialWidth` and `DialHeight` default to 320 and 352 and jointly bound a uniform scale used by measurement, layout and drawing, keeping the dial circular even with different aspect-ratio overrides. `NeedleLength` (68), `NeedleWidth` (48), `RingWidth` (18) and `OutlineThickness` (1.5) use drawing units. The pointer length is clamped to stay clear of direction labels; width and ring spacing are also clamped. Colors, fonts and `WeightMedium`/`WeightRegular` follow dynamic ArcDesign resources.

## Related

Use `Thermometer` for temperature, `MetricCard` for humidity, visibility, pressure or rainfall readings, and `WindRoseChart` for wind statistics.
