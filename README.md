# MRG

## MR多人FPS遊戲

Unity遊戲系統腳本
> 2020/05 – 2021/06

## DEMO

[![MRG DEMO](https://img.youtube.com/vi/nCbOivdA3Gg/0.jpg)](https://youtu.be/nCbOivdA3Gg)

## Detail

由「實體控制器」與「MR遊戲系統」兩個部分組成。「實體控制器」模擬三種武器(自動步槍、狙擊槍、霰彈槍)，提供板機開火、換彈、上膛、槍火閃光與後座力等操作及回饋。玩家透過身體轉向、瞄準與晃動控制遊戲的射擊瞄準；「MR遊戲系統」根據現實環境動態建立MR虛擬環境，提供虛擬物件動態生成，MR射擊效果、多人連線系統、多種武器交互系統與自訂遊戲地圖物件等功能。

> 部分腳本來自[ARCore Depth Lab](https://github.com/googlesamples/arcore-depth-lab)

## 相關技術

- 遊戲操作(藍芽通訊)
- MR系統(ARCore)
- 射擊機制(Raycast)
- 多武器系統(Delegate)
- 虛擬座標定位(座標校準)
- 動態生成物件(物件池+Raycast)
- 多人射擊交互(Network Manager)
- 自訂地圖(永久雲錨+伺服器儲存)
