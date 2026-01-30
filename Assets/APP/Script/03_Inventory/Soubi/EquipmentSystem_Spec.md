# 装備管理システム仕様書 (Equipment System Specification)

現在のUnityプロジェクトにおける装備管理システムの仕組み、データ構造、およびプログラムの解説をまとめたドキュメントです。

---

## 1. システムの全体像 (Architecture)

本システムは、**「データの保持（Model）」**、**「表示の管理（View）」**、**「計算と仲介（Controller/Logic）」**の役割を分割して構成されています。

| コンポーネント | 役割 |
| :--- | :--- |
| **EquipmentData1 (ScriptableObject)** | **データの最小単位**。装備の名前、アイコン、基本ステータス、装備部位（武器・頭など）を定義します。 |
| **InventoryManager (Singleton)** | **永続データストレージ**。シーンを跨いで所持アイテム数や現在の装備状態を辞書型（Dictionary）で保持します。 |
| **Soubikanri.cs (MonoBehaviour)** | **メインコントローラー**。ユーザーのUI操作を受け取り、InventoryManagerのデータを更新し、UIへ反映させます。 |
| **StatusCalc.cs** | **ステータス計算機**。装備が変更された際、ベース値に装備補正値を合算して最終的な能力値を算出します。 |

---

## 2. データの構造と流れ (Data Flow)

### 2.1 データの保持方式
装備情報は以下のC#の**「辞書型 (Dictionary)」**で管理されています。
- `Dictionary<EquipmentData1, int> equipmentCounts`: 「どの装備」を「何個」持っているか。
- `Dictionary<Equipmenttype, EquipmentData1> equipped`: 「どの部位」に「どの装備」を付けているか。

> **文法解説 (Dictionary)**: 
> キー（Key）と値（Value）のペアで管理するコレクション。`equipped[部位] = 装備` と書くだけで、既に何か装備していれば自動的に上書き（交換）されます。

### 2.2 プログラムの実行順序
1. **初期化 (Awake)**: `InventoryManager.Instance` から共通のデータ参照を受け取ります。
2. **フィルタリング (UpdateUI)**: **LINQ**を用いて、全所持アイテムから「現在のタブに合う部位」かつ「所持数 > 0」のものを抽出します。
   ```csharp
   var filtered = equipmentCounts.Where(kv => kv.Value > 0 && kv.Key.GetItemtype() == currentType);
   ```
3. **UI生成**: 抽出したリストに基づき、装備ボタン（Prefab）をスクロールビュー内に生成します。

---

## 3. 主要な機能の仕組み (Core Logic)

### 3.1 装備の装着 (Equip)
ボタン（Eボタン）が押されると以下の手順が走ります。
- `equipped[currentType] = selectedItem;` でデータを更新。
- `ApplyToMiddleSlot()` で画面中央の対応するスロットにアイコンと名前を表示。
- `statusCalc.CalculateTotalStatus()` を呼び出し、プレイヤーの攻撃力などを再計算。

### 3.2 装備の解除 (Unequip)
装備中のアイテムの「解除」または「外す」ボタンが押された時の処理です。
- `equipped.Remove(currentType);` で辞書からデータを削除。
- `ApplyEmptyToMiddleSlot()` を呼び出し、中央スロットに「未装着」のテキストを出し、アイコンを**初期画像（emptySlotSprite）**に差し替えます。

---

## 4. 特徴的なプログラミング技法

### 4.1 ラムダ式 (Lambda Expressions)
ボタンを生成した直後に、クリック時の動作をその場で定義しています。
```csharp
button.onClick.AddListener(() => {
    selected = eq; // クリックされたアイテムを選択状態にする
    UpdateUI();    // 表示を更新する
});
```
これにより、どのボタンがどのアイテムに対応しているかを個別に管理する必要がなくなります。

### 4.2 列挙型 (Enum) の活用
部位（Sword, Head, Body...）を `enum` で定義し、UIのインデックス番号と連動させています。型安全（間違いが起きにくい）なプログラミングを実現しています。

---

## 5. UIの表示ルール

- **装備中表示 (`RefreshEButtons`)**:
  現在装備されているアイテムの横には「E」または「外す」というテキストを表示し、視覚的に判別可能にします。
- **中央スロットの保護**:
  アイコン画像が `null` になった際にUnityの仕様で「白い四角」が出ないよう、画像が未設定の場合は `enabled = false` にするか、あらかじめ指定された背景画像を表示するように制御されています。

---

## 6. バグ防止策 (既知の修正)

- **多段ヒット/宝箱重複防止**:
  `CharaDamage.cs` に `isDead` フラグを導入。死んだ瞬間に `true` にすることで、その後のダメージ判定や死亡時イベント（宝箱生成）が重複して発生するのを完全にブロックしています。
