# PixelViewer 使用者協議
 ---
+ 版本：1.3
+ 更新時間：2022/8/16

這是 PixelViewer 的使用者協議，您應該要在使用 PixelViewer 之前詳細閱讀本協議。 使用者協議可能會在未來有所更新，您可以在 PixelViewer 網站中查看。 當您開始使用 PixelViewer 表示您同意本使用者協議。


## 適用範圍
PixelViewer 為基於開放原始碼專案之軟體，以下所指 PixelViewer **僅包括** 與下列頁面所提供之可執行檔或壓縮檔內容完全相同之版本：

+ [PixelViewer 網站](https://carinastudio.azurewebsites.net/PixelViewer/)
+ [GitHub 上之 PixelViewer 專案頁面及各版本釋出頁面](https://github.com/carina-studio/PixelViewer)

本使用者協議適用於您使用 PixelViewer 2.7 及下一份使用者協議所指定之版本之間 (但不包括) 的所有版本。


## 外部相依性

### X 視窗系統調整及旋轉延伸工具 (XRandR)
在 **Linux** 上偵測顯示設定及套用使用者介面縮放比例。安裝完成後需重新啟動應用程式。


## 檔案存取
除了系統檔案之外，所有 PixelViewer 所需之檔案皆存放於 PixelViewer 目錄內（若您有安裝 .NET 則亦包含 .NET 執行期間 之目錄）。當執行 PixelViewer 且未載入任何圖片時不需要額外的檔案存取，除了下列之外：

+ 讀取 **/proc/meminfo** 以在 **Linux** 上取得記憶體資訊。
+ 讀/寫系統之暫存目錄以存放執行期間所需資源。
+ 其餘由 .NET 或第三方程式庫之必要檔案存取。


### 圖片載入時之檔案存取
+ 包含原始圖片內容之檔案將以 **讀取** 模式開啟。

## 圖片儲存時之檔案存取
+ 寫入圖片內容之檔案將以 **讀寫** 模式開啟。

## 自我升級時之檔案存取
+ 下載的升級檔案及應用程式備份將存放於系統之暫存目錄內。

其他由 PixelViewer 執行檔以外的檔案存取不受本協議之約束。


## 網路存取
PixelViewer 將會在下列狀況存取網路：

### 檢查應用程式更新
PixelViewer 會定期從 PixelViewer 網站下載資訊清單以檢查是否有新的應用程式更新。


### 自我更新
以下 4 種資料需要在更新 PixelViewer 時下載：

+ 自動更新程式之資訊清單以選取適合您的自動更新程式。
+ PixelViewer 之資訊清單以選取適合您的升級封裝。
+ 自動更新程式封裝。
+ PixelViewer 升級封裝。

其他由 PixelViewer 執行檔以外的網路存取不受本協議之約束。


## 執行外部命令
在執行 PixelViewer 時有些必要情況需要執行外部命令：

+ 執行 **dotnet** 以確認在裝置上安裝的 .NET 版本。
+ 執行 **explorer** 以在 **Windows** 上開啟檔案總管。
+ 執行 **open** 以在 **macOS** 上開啟 Finder。
+ 執行 **defaults** 以確認在 **macOS** 上的系統語系與佈景設定。
+ 執行 **nautilus** 或 **xdg-open** 以在 **Linux** 上開啟檔案管理器。
+ 執行 **gnome-shell** 以在 **Linux** 上確認圖形化介面環境。
+ 執行 **xrandr** 以在 **Linux** 上偵測顯示設定及套用使用者介面縮放比例。


## 變更您的電腦
除了檔案存取，PixelViewer **不會** 變更您電腦的設定。


## 授權及著作權
PixelViewer 是 Carina Studio 在 [MIT](https://github.com/carina-studio/PixelViewer/blob/master/LICENSE) 授權之下的開放原始碼專案。除了應用程式圖示外，所有圖示皆在 [MIT](https://github.com/carina-studio/PixelViewer/blob/master/LICENSE) 或 [CC 4.0](https://en.wikipedia.org/wiki/Creative_Commons_license) 授權下使用。您可以在 [MahApps.Metro.IconPacks](https://github.com/MahApps/MahApps.Metro.IconPacks) 了解更多圖示相關資訊與授權。
 
應用程式圖示由 [Freepik](https://www.freepik.com/) 提供並發布於 [Flaticon](https://www.flaticon.com/)。
 
載入至 PixelViewer 或由 PixelViewer 儲存之圖片的授權與著作權不受本協議之約束。您必須自行注意及負責圖片的授權與著作權。


## 聯絡我們
如果您對於本使用者協議有任何疑問，可以至 [GitHub](https://github.com/carina-studio/PixelViewer/issues) 提出或寄信至 [carina.software.studio@gmail.com](mailto:carina.software.studio@gmail.com)。