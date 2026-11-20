---
title: 狀態指示燈
summary: 顯示設備回饋狀態的唯讀指示燈，提供本地化文字和可選閃爍。
source-hash: b9d02a496c7b
---

<Demo name="States" />

## 何時使用

使用 `StatusIndicator` 在上位機中顯示 PLC、閘道器、水泵或馬達的回饋狀態。六種明確狀態為未知、離線、待機、運行、警告和故障。預設顯示狀態文字，顏色不是唯一提示。初始狀態為未知。

## 何時不使用

操作請求使用 `Switch`，簡短中繼資料使用 `Badge`。測量值使用 `Thermometer`、`Gauge` 或圖表。指示燈不會傳送命令、連線設備、查詢資料庫、偵測逾時或確認警報。

## 用法

```xml
<arc:StatusIndicator State="{Binding PumpState}"
                     AutomationProperties.Name="水泵 P-101" />
```

應用程式將收到的回饋對應為 `StatusIndicatorState`，並在 UI 執行緒更新繫結屬性。操作請求與已確認回饋分別保存。根據應用程式策略，將缺測或過期資料對應為 Unknown，將確認的中斷連線對應為 Offline。警報閾值、通訊協定與重新連線邏輯由應用程式負責。

`Label` 可以覆寫狀態文字；null 使用目前語言的狀態名稱。`ShowLabel` 預設為 true；隱藏時在附近提供等價的可見狀態文字。`DisplayText` 支援唯讀繫結。`IsBlinking` 預設為 false；應用程式可按需明確開啟，突出需要關注的狀態。示範展示六種恆亮狀態，以及透過按鈕和既有 Switch 控制項調節的一盞回饋燈。

## API 參考

<!-- api: StatusIndicator -->

## 無障礙

控制項提供目前狀態的唯讀文字值，即使 `ShowLabel` 為 false 也保留。它不接受焦點，也不是切換按鈕。預設無障礙名稱為本地化的「設備狀態」；透過 `AutomationProperties.Name` 標識具體設備。狀態文字變化採用禮貌通知，閃爍不會改變無障礙值。

## 動效

狀態文字和顏色立即更新。可選閃爍只作用於燈，週期為一秒，在 ControlTheme 的 Full 動效選擇器中宣告。Reduced 和 Off 保持恆亮。關閉閃爍或卸載控制項會移除動畫選擇器。沒有每控制項計時器，也不會延遲真實警報。

## 響應式行為

`StatusIndicatorToken.Diameter` 預設 16 個裝置無關像素，`LabelGap` 預設 8。佈局中的燈寬高使用同一直徑，文字採用自然尺寸。主題覆寫動態生效；顏色、圓角、邊框、字型、字級和中等字重來自 ArcDesign 資源。為翻譯後的狀態文字留出空間；僅顯示燈時，在其他可見位置提供狀態文字。

## 相關元件

命令輸入使用 `Switch`，中繼資料使用 `Badge`，溫度使用 `Thermometer`，趨勢使用 `RealtimeStream`。在設備面板中組合這些控制項，由應用程式提供資料。
