# คู่มือ Up Patch เซิร์ฟเวอร์ตามเวอร์ชันเกมใหม่ (Version Upgrade Guide)

> เอกสารนี้คือ runbook สำหรับ "เวลาเกมทางการ (Prod) ออกเวอร์ชันใหม่ เช่น `4.2.0 → 4.3.0` แล้วเราต้องอัปเดต March7thHoney ตามยังไง"
> เขียนอ้างอิงโครงสร้างจริงของ repo นี้ (ตรวจไฟล์/บรรทัดจริง ณ เวอร์ชัน 4.2.0)

---

## TL;DR — เช็กลิสต์สั้น ๆ

เวอร์ชันใหม่ออก ให้ทำ 7 อย่างนี้ตามลำดับ:

1. **สำรองก่อน** — copy `Config/Database/*.db` + commit งาน custom ของเราให้ครบ
2. **อัปเดต Client** ให้เป็นเวอร์ชันใหม่ (ฝั่งเครื่องที่ใช้เล่น)
3. **อัปเดต Proto + CmdIds** (`Proto/`, `KcpSharp/CmdIds.cs`) — ปกติ "merge upstream" ง่ายสุด
4. **อัปเดต Resources/** ให้ตรงเวอร์ชัน (จาก Dimbreath GitLab)
5. **แก้ค่าเวอร์ชัน** ที่ `Common/Util/GameConstants.cs` → `GAME_VERSION`
6. **Build** → `dotnet build Program/Program.csproj -c Release`
7. **Re-apply งาน custom ของเรา** (events / `/cheat` / 99999 gold / stability fixes) แล้ว **smoke test**

> 80% ของงานคือข้อ 3–4. ถ้า `main` ของ upstream อัปเป็นเวอร์ชันใหม่แล้ว → ใช้ทาง **"Merge Upstream"** (ดูด้านล่าง) จะเร็วและพังน้อยที่สุด

---

## ส่วนที่ 1 — เข้าใจก่อน: อะไรบ้างที่ "ผูกกับเวอร์ชันเกม"

เวลาเกมขึ้นเวอร์ชันใหม่ ของที่ "เปลี่ยนตาม" มี 4 ชั้น เรียงจากกระทบมากไปน้อย:

| ชั้น | สิ่งที่เปลี่ยน | ทำไมต้องตรง | ถ้าไม่ตรงจะเป็นยังไง |
|---|---|---|---|
| **A. Protocol** | `Proto/*.cs` + `KcpSharp/CmdIds.cs` | client/server คุยกันด้วย protobuf + opcode ที่ HoYo สับเปลี่ยน/เพิ่มทุกเวอร์ชัน | client ต่อติดแต่ packet เพี้ยน → เด้ง/ค้าง/desync |
| **B. Game data** | `Resources/` (ExcelOutput, Config) | ตัวละคร/แผนที่/ของ/ค่าพลัง อ้างอิงตารางเวอร์ชันนั้น ๆ | ของใหม่หาย, scene/avatar id ไม่เจอ, crash ตอนโหลด |
| **C. Version tags** | `GameConstants.GAME_VERSION`, Hotfix keys | ใช้ตอน handshake / dispatch / watermark | dispatch ไม่ match, hotfix ผิด version |
| **D. Server logic** | handlers ใหม่ + แก้ field ที่ถูก rename | ฟีเจอร์ใหม่ของเวอร์ชันนั้น + proto field obfuscated เปลี่ยนชื่อ | ฟีเจอร์ใหม่ไม่ทำงาน, code เก่า compile ไม่ผ่าน |

### ตารางจุดที่ต้องแตะ (Patch Surface) — อ้างอิงไฟล์จริง

| # | จุด | ไฟล์ / path | ต้องทำอะไร | แหล่งที่มา |
|---|---|---|---|---|
| 1 | **Game client** | (นอก repo) | ติดตั้ง/แพตช์เป็นเวอร์ชันใหม่ | launcher ทางการ / mirror ชุมชน |
| 2 | **Protobuf (.cs)** | `Proto/*.cs` (4,826 ไฟล์, generated) | regenerate จาก `.proto` ใหม่ | ดูข้อ 3 |
| 3 | **Protobuf (source)** | `Proto/ProtoFile/*.proto` | แทนด้วย proto เวอร์ชันใหม่ แล้วรัน `protoc` | LunarCore-Cyt proto repo |
| 4 | **Opcodes** | `KcpSharp/CmdIds.cs` (2,394 บรรทัด) | อัปเดตค่า CmdId ให้ตรงเวอร์ชัน | LunarCore-Cyt / cmdid dump |
| 5 | **Server-only proto** | `ServerSideProto/ProtoFile/*.proto` | มักไม่เปลี่ยน (เป็น proto ของเราเอง) | repo เราเอง |
| 6 | **ค่าเวอร์ชันหลัก** | `Common/Util/GameConstants.cs:5` `GAME_VERSION` | `"4.2.0"` → เวอร์ชันใหม่ | — |
| 7 | **Resources** | `Resources/ExcelOutput`, `Resources/Config` | แทนด้วยข้อมูลเวอร์ชันใหม่ | `gitlab.com/Dimbreath/turnbasedgamedata` |
| 8 | **Avatar DB schema** | `GameConstants.cs:6` `AvatarDbVersion` + `Program/Program/EntryPoint.cs:339,384` | bump **เฉพาะตอน schema avatar เปลี่ยน** (ดูหมายเหตุ) | — |
| 9 | **Hotfix keys** | `Config/Hotfix.json`, `Config/Custom/Hotfix.json` | auto-เติม key ตาม `GAME_VERSION` ให้เอง; ใส่ URL เฉพาะถ้าโฮสต์ asset เอง | — |
| 10 | **งาน custom ของเรา** | events, `/cheat`, 99999 gold, stability fixes | rebase มาวางบนฐานใหม่ | branch ของเรา |

> **อย่าไปยุ่งกับ `GameVersionInt = 3200`** (`GameConstants.cs:7`) เข้าใจผิดบ่อย — มัน**ไม่ใช่เวอร์ชันเกม** แต่เป็น schema เวอร์ชันของ `ServerPrefsData`. ใน `PlayerInstance.cs:223` ถ้าค่านี้ไม่ตรง มันจะ **ล้าง prefs ของผู้เล่นทุกคน** (`ServerPrefsDict.Clear()`). ปล่อยไว้เฉย ๆ ไม่ต้องขยับตามเวอร์ชันเกม — แตะเฉพาะตอนตั้งใจจะ reset prefs เท่านั้น

---

## ส่วนที่ 2 — เลือกเส้นทาง: Merge Upstream vs ทำเอง

มี 2 ทางในการได้ Proto/CmdIds/Resource เวอร์ชันใหม่:

### ทาง A — Merge Upstream (แนะนำ ✅ เร็ว/พังน้อย)
repo นี้เป็น fork; โปรเจกต์ต้นน้ำ (March7thHoney / DanhengServer / LunarCore-Cyt) จะอัป Proto + CmdIds + handler ของเวอร์ชันใหม่ให้ เราแค่ดึงมาแล้ว rebase งาน custom ทับ

```powershell
# ครั้งแรกครั้งเดียว: ผูก remote ต้นน้ำ
git remote add upstream https://github.com/Mar7thLover/March7thHoney.git

# เวลามีเวอร์ชันใหม่
git fetch upstream
git checkout main
git merge upstream/main          # ได้ proto/cmdids/resource-loader/handler ใหม่
# จากนั้นไป "ส่วนที่ 6" เพื่อ rebase งาน custom ของเรา
```

### ทาง B — ทำเอง (ตอน upstream ยังไม่อัป หรืออยากนำหน้า)
ต้อง regenerate proto + cmdids เอง (ดู "ส่วนที่ 4"). ใช้เวลามากกว่า เหมาะตอนอยากเล่นเวอร์ชันใหม่ก่อนต้นน้ำ

> **กฎเหล็ก:** Proto, CmdIds, Client และ Resources ต้องเป็น **เวอร์ชันเดียวกันทั้งหมด** ปนเวอร์ชันเมื่อไหร่ = เด้ง/desync ทันที

---

## ส่วนที่ 3 — ขั้นตอนละเอียด

### 3.0 Pre-flight (ทำทุกครั้ง ห้ามข้าม)

```powershell
# 1) สำรอง database (กัน save ผู้เล่นหาย/พังตอน migrate)
Copy-Item "Config\Database\March7thHoney.db" "Config\Database\March7thHoney.db.bak-4.2.0"

# 2) เช็กว่างาน custom เรา commit ครบ ไม่มีอะไรค้าง
git status

# 3) จำเวอร์ชันที่ใช้ได้ดีไว้ (เผื่อ rollback)
git tag working-4.2.0
```

### 3.1 อัปเดต Client
- อัปเดต/แพตช์ตัวเกมบนเครื่องที่ใช้เล่นให้เป็นเวอร์ชันใหม่ แล้วจดเลข `game_version` (เช่น `4.3.0`)
- เลขนี้ต้องตรงกับ `GAME_VERSION` ในข้อ 3.4

### 3.2 อัปเดต Resources/
`Resources/` ถูก `.gitignore` (ไม่อยู่ใน repo) ดึงจาก Dimbreath:

```powershell
# branch main ของ Dimbreath = เวอร์ชันล่าสุดเสมอ
git clone --depth 1 https://gitlab.com/Dimbreath/turnbasedgamedata.git Resources
```

- ถ้า client ยังเป็นเวอร์ชันเก่ากว่า `main` ของ Dimbreath → checkout commit ที่ขึ้นต้นด้วย `OSPRODWin<version>_*` ให้ตรง
- เช็กว่ามีโฟลเดอร์ครบ: `ExcelOutput/`, `Config/LevelOutput/`, `Config/Level/Mission`, `Config/Level/Rogue`, `Config/Gameplays/RogueDLC`, `ConfigCharacter` (ตัวโหลดอยู่ใน `ResourceManager`)

### 3.3 อัปเดต Protocol (Proto + CmdIds)
- **ทาง A (merge upstream):** ได้มาอัตโนมัติตอน merge แล้ว ข้ามไปข้อ 3.4
- **ทาง B (ทำเอง):** ดู "ส่วนที่ 4"

### 3.4 แก้ค่าเวอร์ชัน — `Common/Util/GameConstants.cs`

```csharp
public const string GAME_VERSION = "4.2.0";   // ← แก้เป็นเวอร์ชันใหม่ เช่น "4.3.0"
```

ผลที่ตามมาอัตโนมัติเมื่อแก้บรรทัดนี้:
- `ConfigManager.LoadHotfixData()` จะ **เติม key `CN<ver>` / `OS<ver>` ใน `Config/Hotfix.json` ให้เอง** ตอนบูต (ไม่ต้องแก้มือ)
- Watermark + handshake (`Connection.cs:60` `WIN{GAME_VERSION}`) อัปตาม
- หน้า console ตอนบูตจะโชว์เวอร์ชันใหม่

> หมายเหตุ logic เล็ก ๆ: ถ้าเวอร์ชันลงท้ายด้วย `5` (เช่น beta `x.y.5`) `LoadHotfixData()` จะสร้าง key ย่อย `...1`..`...5` ให้ — เป็นพฤติกรรมปกติของ patch beta ไม่ต้องแก้อะไร

### 3.5 Avatar DB migration (เฉพาะกรณี schema เปลี่ยน)
`EntryPoint.cs:337-394` มี migration ที่จะ "แปลงข้อมูล avatar เก่า → รูปแบบใหม่" เมื่อ `avatarData.DatabaseVersion != GameConstants.AvatarDbVersion`

- **ปกติเวอร์ชันเกมใหม่ ไม่ต้องแตะตรงนี้** (`AvatarDbVersion` ผูกกับ schema ของ "เรา" ไม่ใช่เวอร์ชัน HoYo)
- แตะเฉพาะตอน **เราเปลี่ยนโครงสร้าง `AvatarData` เอง** เท่านั้น แล้วถ้าแตะ ให้แก้ทั้ง 2 จุดให้ตรงกัน:
  - `GameConstants.cs:6` → `AvatarDbVersion = "วันที่ใหม่"`
  - `EntryPoint.cs:384` → `avatarData.DatabaseVersion = "วันที่ใหม่"` (⚠️ ตอนนี้ hardcode `"20250430"` ไม่ได้อ้าง constant — ถ้าจะ migrate รอบใหม่ ควรเปลี่ยนเป็น `GameConstants.AvatarDbVersion` ทั้งคู่กันลืม)

### 3.6 Build

```powershell
dotnet build Program/Program.csproj -c Release
```

ถ้า error ส่วนใหญ่จะเป็น **proto field ถูก rename** → ดู "ส่วนที่ 5"

### 3.7 Re-apply งาน custom + Test
ไป "ส่วนที่ 6" และ "ส่วนที่ 7"

---

## ส่วนที่ 4 — Regenerate Proto + CmdIds (ทาง B เท่านั้น)

Proto ใน repo นี้เป็น `.cs` ที่ **generate ไว้ล่วงหน้าแล้ว commit** (Proto.csproj อ้างแค่ `Google.Protobuf`, ไม่มี Grpc.Tools auto-compile) ตัว source อยู่ที่ `Proto/ProtoFile/*.proto`

### 4.1 ต้องมี `protoc`
ติดตั้ง Protocol Buffers compiler ให้เรียก `protoc` ได้จาก PATH

### 4.2 หา proto เวอร์ชันใหม่
proto ของ HSR ชื่อ field ถูก **obfuscate** (เช่น `LMBHDCFPPLL`, `BMKAEFAKNFJ`) และเปลี่ยนทุกเวอร์ชัน — ต้องได้ชุดที่ dump มาตรงเวอร์ชัน แหล่งที่ repo นี้ใช้คือ **LunarCore-Cyt** (ดูสคริปต์ `Proto/RefreshProtoFromLunarCoreCyt.ps1`)

### 4.3 รัน refresh script
```powershell
# ชี้ไปโฟลเดอร์ proto เวอร์ชันใหม่ — script จะ copy, ใส่ csharp_namespace, แล้วรัน protoc สร้าง .cs ใหม่
.\Proto\RefreshProtoFromLunarCoreCyt.ps1 -SourceProtoDir "D:\path\to\new-version\proto"
```
สคริปต์ทำให้: copy `.proto` เข้า `ProtoFile/` → inject `option csharp_namespace = "March7thHoney.Proto";` → ลบ `.cs` เก่า → `protoc --csharp_out=..` สร้างใหม่

proto ฝั่งเซิร์ฟเวอร์เอง (custom 2 ไฟล์) regenerate แยกด้วย `ServerSideProto/RefreshProto.bat` — **มักไม่ต้องทำ** เพราะเป็น proto ของเราไม่ผูกเวอร์ชัน

### 4.4 อัปเดต `KcpSharp/CmdIds.cs`
opcode (CmdId) ก็เปลี่ยนตามเวอร์ชัน เอาค่าใหม่จากชุด dump เดียวกับ proto มาแทนใน `CmdIds.cs` ค่าที่ไม่ตรง = handler ถูกเรียกผิดตัว/ไม่ถูกเรียก

> เพราะข้อ 4.2–4.4 ยุ่งและพลาดง่าย — **ทาง A (merge upstream) จึงคุ้มกว่าเกือบทุกครั้ง**

---

## ส่วนที่ 5 — แก้ code ที่พังหลัง bump (proto field rename)

หลังเปลี่ยน proto, code เก่าที่อ้าง field obfuscated ที่ "หายไป/เปลี่ยนชื่อ" จะ compile ไม่ผ่าน วิธีไล่:

1. Build แล้วดู error `CS0117 / CS1061` (ไม่มี member ชื่อนั้น)
2. หาชื่อ field ใหม่: เปิด `Proto/<MessageName>.cs` แล้ว grep array `Parser, new[]{ ... }` (ลำดับ field) เทียบกับ proto เก่า — field ที่ field-number เดิมคือ "ตัวเดียวกันที่เปลี่ยนชื่อ"
3. หา type ของ field: `grep "public .* <FIELD> {"` ใน `Proto/<MessageName>.cs`
4. แก้ชื่อใน code เราให้ตรง

> นี่คือเหตุผลที่ field obfuscated ทำให้ HSR private server ต้อง maintain ต่อเวอร์ชัน — ดู `docs/GameplayModuleNameMap.md` ประกอบ (map ชื่อ module เช่น Currency War = `GridFight`)

---

## ส่วนที่ 6 — Re-apply งาน custom ของเรา (สำคัญ)

งาน custom ที่เราทำไว้ ต้องเอามาวางบน "ฐานเวอร์ชันใหม่" ทุกครั้ง:

| ฟีเจอร์ | ไฟล์หลัก |
|---|---|
| Events: SwordTraining, AetherDivide, Heliobus | `GameServer/Game/Activity/Activities/*`, `Common/Database/Activity/*`, handlers/packets |
| `/cheat` (God Mode / Max Energy) | `Command/.../CommandCheat.cs`, `PlayerData.cs`, `BattleManager.cs`, `AvatarData.cs` |
| Currency War 99999 gold | `GameServer/Game/GridFight/GridFightInstance.cs` |
| Stability fixes (HP/SP clamp ฯลฯ) | settlement / battle code |

### วิธีที่แนะนำ: เก็บ custom เป็น branch แยก แล้ว rebase
```powershell
# หลัง main ได้เวอร์ชันใหม่แล้ว
git checkout feature/events-and-stability
git rebase main          # เอา commit custom เราไปวางบน main ใหม่
# แก้ conflict (มักเป็นจุดที่ proto field เปลี่ยน) → git add → git rebase --continue

git checkout feature/aetherium-heliobus
git rebase main
```

- **เก็บ feature เป็น commit แยกเรื่อง** (events / cheat / gold คนละ commit) → rebase แล้ว conflict ทีละเรื่อง แก้ง่ายกว่า
- งานที่ยัง **uncommitted** (เช่น 99999 gold) **commit เข้า branch ก่อน** ไม่งั้นหายตอน merge/rebase
- pattern การทำ event อยู่ใน `docs/` (ดู memory `event-module-pattern`) — ถ้า proto event เปลี่ยน ใช้วิธี "ส่วนที่ 5" ไล่ field ใหม่

---

## ส่วนที่ 7 — Smoke Test หลัง patch

รันเซิร์ฟเวอร์ + client เวอร์ชันใหม่ แล้วไล่เช็ก:

- [ ] Server บูตขึ้น ไม่มี exception ตอนโหลด Resources (ดู log ที่ `Config/Logs/`)
- [ ] Console โชว์เวอร์ชันใหม่ถูกต้อง
- [ ] Client ต่อติด ผ่าน dispatch/gateway เข้าเกมได้ (ไม่เด้งหน้า login)
- [ ] เข้าเกม: ตัวละคร/lineup โหลดครบ ไม่หาย
- [ ] เข้า battle จบได้ HP/SP ไม่ desync (ทดสอบ stability fix)
- [ ] Gacha/Warp ปกติ
- [ ] ฟีเจอร์ custom: `/cheat` ทำงาน, Currency War เงินเริ่ม 99999, event panels เปิดได้
- [ ] ของใหม่ของเวอร์ชันนั้น (ตัวละคร/แมพ/event ใหม่) โผล่
- [ ] `/scene reload` แก้ปัญหา scene เพี้ยนได้ (ตามที่ README บอก)

ถ้าพฤติกรรมแปลก ๆ หลังอัป: ลองล้าง runtime cache + ทดสอบด้วย account ใหม่ก่อน (ตาม README → Notes)

---

## ส่วนที่ 8 — Troubleshooting

| อาการ | สาเหตุที่น่าจะเป็น | แก้ |
|---|---|---|
| Client เด้งตอน login / connect | Proto หรือ CmdIds ไม่ตรงเวอร์ชัน client | sync Proto+CmdIds+Client ให้เวอร์ชันเดียวกัน |
| Server crash ตอนบูต โหลด Resources | Resources ผิดเวอร์ชัน/ไม่ครบ | clone Resources ให้ตรงเวอร์ชัน เช็กโฟลเดอร์ครบ |
| Compile error `CS1061/CS0117` หลังเปลี่ยน proto | field obfuscated ถูก rename | ไล่ field ใหม่ตาม "ส่วนที่ 5" |
| เข้าเกมได้แต่ packet เพี้ยน/บางอย่างค้าง | CmdId บางตัวไม่ตรง | ตรวจ `KcpSharp/CmdIds.cs` |
| ตัวละคร/save หายหลังอัป | DB migration ทำงานผิด หรือเผลอ bump prefs/db version | restore `.db.bak`, ตรวจข้อ 3.5 / `GameVersionInt` |
| prefs ผู้เล่นโดน reset เอง | เผลอแก้ `GameVersionInt` | คืนค่าเดิม (ดูหมายเหตุส่วนที่ 1) |
| ของใหม่เวอร์ชันนั้นไม่โผล่ | Resources ยังเวอร์ชันเก่า / handler ใหม่ยังไม่มี | อัป Resources + merge handler ใหม่จาก upstream |

---

## ภาคผนวก — Reference เร็ว

- **เวอร์ชันปัจจุบัน:** `4.2.0` (`Common/Util/GameConstants.cs:5`)
- **Build:** `dotnet build Program/Program.csproj -c Release`
- **Resources:** `https://gitlab.com/Dimbreath/turnbasedgamedata` (branch `main` = ล่าสุด)
- **Proto source:** `Proto/ProtoFile/*.proto` → regenerate ด้วย `Proto/RefreshProtoFromLunarCoreCyt.ps1`
- **Opcodes:** `KcpSharp/CmdIds.cs`
- **Dispatch/hotfix logic:** `WebServer/Handler/QueryGatewayHandler.cs`, `Common/Util/ConfigManager.cs` (`LoadHotfixData`)
- **DB migration:** `Program/Program/EntryPoint.cs:337-394`
- **Module name map (Currency War=GridFight ฯลฯ):** `docs/GameplayModuleNameMap.md`
- **Community:** <https://discord.gg/castoriceps>

> สรุปหลักคิด: **เวอร์ชันเกม = ชั้น Protocol + Data + Client ต้องตรงกันเป๊ะ** ส่วน code/feature ของเราคือสิ่งที่เอามา "วางทับ" ฐานเวอร์ชันใหม่ทุกรอบ จัดงาน custom เป็น branch/commit แยกเรื่องไว้ → ทุกครั้งที่เวอร์ชันใหม่มา แค่ merge upstream + rebase งานเรา + อัป Resources + แก้ `GAME_VERSION` + build
