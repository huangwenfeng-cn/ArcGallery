---
title: 風向指示器
summary: 即時風向羅盤，提供可選方向讀數以及明確的靜風、無資料狀態。
source-hash: 45f328a458b2
---

<Demo name="Readings" />

## 何時使用

用 `WindDirectionIndicator` 顯示 HMI 或氣象面板中的目前氣象風向。實心指針指向風的來向：北為 0°、東為 90°、南為 180°、西為 270°。雙圓環之間形成刻度帶，外側完整顯示 16 個角度標籤，間隔 22.5°；內側顯示八方位，四主方位更大，配有四個三角定位標記。可選讀數顯示最近的十六方位名稱。

## 何時不用

歷史風向和風速分布使用 `WindRoseChart`，風速趨勢使用 `LineChart` 或 `RealtimeStream`。羅盤負責顯示測量值，不選擇方向或執行設備控制。

## 用法

```xml
<arc:WindDirectionIndicator Direction="22.5" ShowReading="True" />
```

將 `Direction` 綁定到檢視模型，並在 UI 執行緒更新。有限角度正規化到 [0, 360)：360 表示北，-90 表示西。`ShowReading` 預設 false；開啟後在羅盤下方顯示角度與方位名稱。可見標題和風速放在外層面板，風速可使用獨立 `MetricCard` 等讀數元件。

應用程式判定靜風時設定 `IsCalm`：隱藏指針，在圓心顯示靜風文字。取樣不可用時設定 `HasData=false`：無資料狀態優先於靜風，隱藏指針，在圓心顯示無資料文字。即使關閉 `ShowReading`，狀態文字仍可見，狀態切換不改變羅盤尺寸。收到有效取樣後恢復狀態標記。靜風閾值和取樣品質由應用程式處理。

`Label` 覆寫無障礙名稱，`CalmLabel`、`NoDataLabel` 覆寫狀態文字；null 時跟隨應用程式語言。方位名稱支援英文、簡體中文和繁體中文。範例包含一個羅盤、方向滑桿、兩個跨北向按鈕、靜風和無資料按鈕，以及方向讀數開關。移動滑桿會恢復測量讀數。

## API 參考

<!-- api: WindDirectionIndicator -->

## 無障礙

羅盤提供唯讀文字值，包括實際角度和方位名稱，或明確的靜風、無資料狀態；隱藏可視讀數不影響無障礙讀數。本地化元件名稱作為預設無障礙名稱，`Label` 可覆寫，應用程式明確設定的自動化名稱優先。控制項不接收焦點或使用者輸入。

## 動效

只有箭頭使用可中斷的 Arc 彈簧，從目前視覺位置沿最短路徑旋轉：359° 到 1° 只跨越北向 2°。文字立即更新。Reduced 和 Off 模式直接到達目標。靜風、無資料或重新掛載後的首個有效方向立即顯示。

## 響應式行為

預設繪圖參考座標為 320 × 320，開啟 `ShowReading` 後為 320 × 352。`WindDirectionIndicatorToken.DialWidth` 和 `DialHeight` 預設 320、352，共同限制最大等比例顯示尺寸，測量、配置和繪製共用縮放規則，因此寬高覆寫比例不同也保持圓盤為圓形。`NeedleLength`（68）、`NeedleWidth`（48）、`RingWidth`（18）、`OutlineThickness`（1.5）使用繪圖單位。指針長度受限以避開方位文字，寬度和刻度帶間距也有範圍限制。色彩、字型及 `WeightMedium`、`WeightRegular` 跟隨動態 ArcDesign 資源。

## 相關控制項

溫度使用 `Thermometer`，濕度、能見度、氣壓和降雨量讀數使用 `MetricCard`，風向統計使用 `WindRoseChart`。
