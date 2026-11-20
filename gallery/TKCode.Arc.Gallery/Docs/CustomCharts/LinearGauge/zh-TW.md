---
title: 線性儀表
summary: 以直向柱條展示拉力、壓力、轉速等連續量程讀值。
source-hash: e25d02cc4c00
---

<Demo name="Readings" />

## 何時使用

使用 `LinearGauge` 展示連續數值在量程中的位置。窄柱條表達比例，實際數字、單位及明確的閾值狀態保持可見。示範並排展示拉力、壓力與轉速。

## 何時不使用

儲槽或容器使用 `LevelIndicator`，圓形儀表使用 `Gauge`，歷史趨勢使用 `LineChart`。設定目標值使用滑桿。本控制項唯讀，不擷取資料或控制設備。

## 用法

```xml
<arc:LinearGauge Label="拉力" Unit="N" Value="450"
                 Minimum="0" Maximum="1000"
                 LowThreshold="200" HighThreshold="800" Decimals="0" />
```

數值與閾值使用同一單位。`Unit` 只顯示文字，不轉換數值。低於 `LowThreshold` 為 `Low`，達到或超過 `HighThreshold` 為 `High`，中間區間為 `Normal`。`Decimals` 控制實際數字的小數位（0～6，預設 1）。預設量程 0～100，閾值 20/80。

將 `Value` 與 `Quality` 繫結到應用程式資料，設備更新應切換到 UI 執行緒。有限越界讀值仍顯示實際數字，柱條停在邊界。上下限相等或倒置、閾值順序倒置時顯示 `InvalidRange` 並清空軌道。拒絕 NaN 與無窮值。

`Quality` 預設 `Good`。`NoData`、`Stale`、`Error` 隱藏柱條並以破折號取代數字；品質文字優先於閾值與量程錯誤文字。保存的 `Value` 與 `State` 不變。使用保存讀值前先檢查品質。時間戳記、逾時及感測器診斷由應用程式負責。先更新數值，再恢復 `Good`。

`LowTone`、`NormalTone`、`HighTone` 預設 Warning、Accent、Danger。軌道左側短標記表示量程內的閾值。`LowLabel`、`NormalLabel`、`HighLabel`、`NoDataLabel`、`StaleLabel`、`ErrorLabel` 可覆寫文字；null 跟隨應用程式語言。`ShowScale` 控制刻度顯示，`TickCount` 設定 1～10 個分段。

示範的滑桿與按鈕只改變拉力。品質按鈕更新繫結的資料來源；讀值按鈕即使重複相同數值也恢復 Good。壓力與轉速是獨立範例。

## API 參考

<!-- api: LinearGauge -->

## 無障礙

控制項提供唯讀數值範圍與唯讀文字，以 `Label` 為無障礙名稱。說明文字描述實際讀值與狀態；非 Good 時只報告品質。數值介面保留保存的、限制到量程內的數值，使用方應先檢查品質再將其視為目前值。控制項沒有互動操作。

## 動效

只有連續 Good 讀值的柱條高度使用可中斷的 Arc 彈簧。數字、狀態與顏色立即更新。Reduced、Off、首個有效樣本、品質恢復及重新掛載均直接落到目標。顏色、圓角、字型與字重跟隨動態 ArcDesign 資源。

## 響應式行為

參考座標為 220 × 360。`LinearGaugeToken.DialWidth` 與 `DialHeight` 限制整體等比縮放，更小容器進一步縮小。兩者設為 440 × 720 可顯示兩倍尺寸，440 × 360 仍顯示 220 × 360。`TrackWidth`（預設 36，限制 16～64）與 `OutlineThickness`（預設 1.5，限制 0～4）使用參考座標單位。軌道圓角跟隨 `RadiusControl`。窄軌道旁可完整顯示四位刻度數字；極大或極小刻度使用科學記數法。

## 相關

容器充滿程度使用 `LevelIndicator`，溫度使用 `Thermometer`，圓形量程儀表使用 `Gauge`，連續樣本使用 `RealtimeStream`。
