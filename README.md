# 分散メッセージングアプリケーション

[![Product Name](https://raw.github.com/GabLeRoux/WebMole/master/ressources/WebMole_Youtube_Video.png)](https://www.youtube.com/channel/UC4PtjOfZTbVp9DwtJv82Lzg)

## 製品概要
### Communicate Tech
社内など、限定的な空間でのメッセージングのセキュリティ強化、および効率化を実現します。

### 背景（製品開発のきっかけ、課題等）
現状メッセンジャーツール・チャットツールは、ほぼインターネット上にある外部のサーバを経由して実現されています。
しかし、社内での利用など、セキュリティが重要な場面では、機密情報の流出や改ざんを懸念し、メッセンジャーの導入を断念する場合が多いです。

そこで、インターネットを利用しないメッセンジャーを作ってセキュリティの向上を図るため、このアプリケーションを開発しました。

### 製品説明（具体的な製品の説明）
本メッセンジャーは*インターネットやサーバに頼らない*新発想のアプリです。
仕様やAPIを公開する予定なので、様々なサービスやデバイスと連携することも可能です。

### 特長
#### 特長1　可能な限り「直接通信」にこだわったメッセンジャー
相手とは主にBluetooth LEを用いて通信します。
電波範囲外の場合は、LAN内ブロードキャストやローカルサーバ（後述）を経由して通信することもできます。
インターネットを経由しないため、外部の攻撃者に狙われるリスクや、インターネット回線の負荷を軽減します。

#### 特長2　「緊急度設定」「スレッド」機能により、重要なやり取りを判別しやすい
話題ごとにスレッド分けできる機能を持ち、巨大なグループ内で複数の話題が並行し、煩雑になってしまうという心配もありません。
メッセージには*緊急　普通　トーク*の３つの*緊急度*を設定できます。
緊急度によって通知方法を変えることで、緊急連絡を見逃す可能性や、雑談などの優先度が低いメッセージに埋もれる可能性を軽減します。

#### 特長3　誰でもサーバを作れる・その場所だけのグループが作れる
メッセージを送受信できる場所を限定できるので、その場所だけの秘密のやり取りなども安心して行えます。
マルチプラットフォームで動作するサーバアプリケーションを提供するので、専門知識無しで誰でも「ローカルサーバ」を構築できます。
基本的にサーバはクライアントのリストを管理するのみで、できるだけメッセージを中継せずにクライアント間で直接通信させるため、高いスペックを必要としません。
※ローカルサーバは現時点で未実装です。

### 解決出来ること
インターネットが使えなくても連絡を取り合うことができるようになります。
例えば、災害等でインターネットが使えなくなっても、一定距離までであれば通信可能です。

また、サービス運営者にかかるコストを大幅に削減できます。
誰でもサーバーを立てることができるので、運営者に通信が集中しません。

### 今後の展望
1. メッセージフィルタリング機能の追加
2. 誰でも構築できるローカルサーバの実装
3. 相手が遠くにいる場合はインターネット経由でもやり取りできるようにする
4. ファイル送信機能や自作スタンプ機能の追加
5. ビジネス文書作成支援機能の追加
6. 音声通話・ビデオ通話機能の追加
7. IP電話機器に本アプリと通信できる機能を組み込む
8. IPネットワークを一切使わず、Bluetoothのみでメッセージを送受信できる機能
    自分と相手の間にいる、知り合いのデバイスがメッセージを中継することで実現する
9. 本アプリでIoTデバイスにメッセージを送って、IoTデバイスを制御できるようにする

## 開発内容・開発技術
### 活用した技術
主に「分散システム」の技術を応用して開発しています。

#### API・データ
未使用 

#### フレームワーク・ライブラリ・モジュール
* Universal Windows Platform
* .NET Standard

#### デバイス
* Windows10が動作するPC/モバイルデバイス/IoTデバイス


### 独自開発技術（Hack Dayで開発したもの）
#### 2日間に開発した独自の機能・技術
* メッセージにデジタル署名を付加する機能
CommonLibrary/DMessenger/MessageEncoder.csで実装されています。
* Bluetoothを用いたメッセージ配信機能
* LANにメッセージをブロードキャストする機能
WinMessenger/MessageTransfer.csで実装されています。
