---
title: 溫度計
summary: 唯讀溫度儀表，包含液柱、刻度、實際讀數與明確的高低溫狀態。
source-hash: dc222a6e8299
---

<Demo name="Readings" />

<Demo name="QualityStates" />

## 何時使用

用 `Thermometer` 顯示設備、房間、冷庫或加熱器的目前溫度。數字保留真實讀數，液柱表示它在量程中的位置；狀態文字協助使用者判斷高低溫，無需僅依賴顏色。

## 何時不用

歷史與趨勢使用 `LineChart` 或 `RealtimeStream`。設定溫度使用輸入框或滑桿；溫度計只負責顯示讀數。警報與控制邏輯由應用程式負責，不能依據動畫中的液柱高度執行控制。

## 用法

`Value`、`Minimum`、`Maximum`、`LowThreshold` 與 `HighThreshold` 必須使用相同的溫度單位。讀數小於低溫閾值為 `Low`，大於等於高溫閾值為 `High`，中間為 `Normal`。單位文字不會自動轉換溫度。

```xml
<arc:Thermometer Label="设备温度" Value="36.5"
                 Minimum="-20" Maximum="100"
                 LowThreshold="10" HighThreshold="70" />
```

將 `Value` 綁定到檢視模型中的測量值，並在 UI 執行緒更新。越界讀數仍完整顯示，液柱停在量程邊界。量程顛倒、上下限相等或閾值顛倒時，顯示 `InvalidRange` 並清空液柱；不接受 NaN 或無窮值。

將 `Quality` 綁定到應用程式提供的 `MeasurementQuality`：`Good`（預設）、`NoData`、`Stale` 或 `Error`。非 Good 時隱藏液柱，目前數字替換為破折號；品質文字優先於高低溫及量程錯誤文字。`Value` 和閾值 `State` 仍保留，使用保存值前應檢查品質。時間戳記、逾時規則和感測器診斷由應用程式負責，控制項不輪詢。恢復時先更新數值，再設定 `Good`。

`NoDataLabel`、`StaleLabel`、`ErrorLabel` 覆寫品質文字；null 時跟隨應用程式語言。互動範例將 `Quality` 綁定到可觀察的取樣資料來源，增加品質與恢復按鈕。移動滑桿或點擊溫度按鈕恢復 `Good`，同值按鈕也能恢復。

`LowTone`、`NormalTone` 與 `HighTone` 選擇既有的 Arc 色調。三種狀態文字可透過 `LowLabel`、`NormalLabel`、`HighLabel` 覆寫，為 null 時跟隨應用程式語言。`ShowScale` 控制刻度顯示，`TickCount` 控制分格數。

範例只保留一個溫度計，滑桿和溫度按鈕更新同一個讀數。達到量程上限時，液體會完整填充頂部圓弧；液面與刻度共用相同的內部高度。

## API 參考

<!-- api: Thermometer -->

## 無障礙

溫度計提供唯讀範圍值，以標籤作為無障礙名稱，說明文字包含實際讀數、單位與狀態。控制項不接收焦點或使用者輸入。`State` 立即變更，與液柱動畫無關。

非 Good 時，說明文字和唯讀文字值只報告品質狀態。為保持相容，原數值範圍介面仍回傳保存的、限制到量程內的數值；使用者應檢查品質後再視為目前值。

## 動效

僅液柱使用可中斷的 Arc 彈簧。數字、顏色與狀態始終描述實際讀數。Reduced 與 Off 模式立即到達目標。切換主題時顏色與字型自動跟隨 ArcDesign 令牌；標題和讀數使用 `WeightMedium`，狀態和刻度使用 `WeightRegular`。

品質變更立即生效並停止液柱動效。品質恢復或重新掛載後的首個讀數直接到達目標。

## 響應式行為

繪圖參考座標為 220 × 360。`ThermometerToken.DialWidth` 與 `DialHeight` 共同限制最大顯示尺寸，測量和繪製採用相同的等比例縮放規則，因此寬高權杖比例不同也不會拉伸球泡。兩者設為 440 × 720 時允許顯示兩倍大小的儀表；設為 440 × 360 時仍適配為 220 × 360。較小容器會進一步縮小顯示。其餘元件權杖以繪圖參考座標單位控制管寬、球泡半徑和描邊厚度。

## 相關控制項

通用量程儀表使用 `Gauge`，持續接收溫度樣本使用 `RealtimeStream`，歷史溫度使用 `LineChart`。
