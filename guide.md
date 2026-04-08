# Isometric Dash 项目指南

`Isometric Dash` 是这款 Unity 3D 康复跑酷游戏的完整项目名。本文档基于仓库当前内容整理，重点帮助接手者快速理解这款游戏现在做到了什么、怎么运行、核心逻辑在哪，以及哪些地方还处在原型阶段。

## 1. 项目定位

这不是一个单纯的休闲跑酷 Demo，而是一个“康复训练包装成轻量跑酷流程”的 Unity 原型项目。

从当前代码和配置来看，项目想要承载的是：

- 面向中风患者的腕关节康复训练背景
- 通过持续前进、路径保持和方向修正来模拟训练过程中的专注与控制
- 通过胜利、失败、暂停、主菜单、音量设置等常规游戏反馈降低训练门槛
- 保留外部硬件、UDP、MySQL 的联调能力，便于继续接入康复系统

当前版本的“主玩法”已经能独立跑通，但“康复硬件链路”更像是被保留在工程里的扩展层，还没有直接驱动玩家主控脚本。

## 2. 技术栈与依赖

### Unity 环境

- Unity：`2022.3.53f1c1`
- Render Pipeline：`Universal Render Pipeline 14.0.11`
- UI：`UGUI` + `TextMeshPro`
- 目标平台：`Windows`

### 第三方依赖

仓库内已包含：

- `Assets/Plugins/MathNet.Numerics.dll`
- `Assets/Plugins/MySql.Data.dll`

代码中还显式依赖但不一定随仓库一起可用：

- `PCANBasic.dll`

这意味着只跑游戏原型时主要依赖 Unity 本身；如果要联调编码器 / CAN 设备，则需要额外补齐 `PEAK PCAN` 的驱动和运行库。

## 3. 仓库结构

```text
Isometric/
├─ Assets/
│  ├─ Plugins/                  # 数值计算 / MySQL DLL
│  ├─ Resources/                # 模型、预制体、UI、背景等资源
│  ├─ Scenes/
│  │  ├─ Start.unity
│  │  └─ SampleScene.unity
│  ├─ Scripts/
│  │  ├─ Entity/               # 数据结构与表对象
│  │  ├─ QtMysqlUDP/           # 配置、日志、UDP、MySQL 封装
│  │  ├─ ScriptsUDP/           # 编码器 / CAN / 样例通信脚本
│  │  └─ 玩法与 UI 脚本
│  └─ Sounds/
├─ config/
│  └─ config.ini
├─ docs/
│  └─ images/
├─ Packages/
├─ ProjectSettings/
├─ README.md
└─ guide.md
```

整体上可以分成三层：

- 可直接游玩的跑酷玩法层
- 菜单 / UI / 音频 / 日志等通用应用层
- 康复设备、数据库、UDP 联调层

## 4. 场景结构与运行流程

### 4.1 Build Settings

当前启用的场景只有两个：

- `Assets/Scenes/Start.unity`
- `Assets/Scenes/SampleScene.unity`

### 4.2 Start 场景

`Start.unity` 不是一个只有按钮的空壳菜单，它同时承担了全局对象初始化职责。场景里能看到的关键对象包括：

- `Main Camera`
- `GameManager`
- `Config`
- `Log`
- `EventSystemManager`
- `Encoder`

主菜单逻辑由 `MainMenuController` 负责，功能包括：

- 点击开始进入 `SampleScene`
- 打开设置面板
- 切换菜单 BGM / 游戏 BGM
- 退出程序

设置面板由 `SettingsController` 负责，当前主要实现是：

- 音量 Slider 调整
- 通过 `AudioMixer` + `AudioListener` 生效
- 使用 `PlayerPrefs` 保存音量

### 4.3 SampleScene 场景

`SampleScene.unity` 里能看到以下关键对象或系统：

- `Main Camera`
- 绑定了 `PlayerMovement` 的玩家对象
- `PlatformSpawner`
- 背景对象
- `WinPanel`
- 暂停按钮与暂停面板
- `Encoder2`

运行流程可以概括为：

1. 从 `Start.unity` 点击开始按钮进入 `SampleScene`
2. 玩家自动向前移动
3. 通过鼠标左右移动修正前进方向
4. 平台持续补充，离开的平台会下落
5. 玩家掉落则失败，分数达到目标值则胜利
6. 可通过 UI 返回主菜单或重新开始

## 5. 当前主玩法是怎么运作的

### 5.1 玩家控制

主控脚本是 `Assets/Scripts/PlayerMovement.cs`。

当前版本的输入方式非常明确：

- 使用 `Input.GetAxis("Mouse X")` 读取鼠标横向移动
- 通过鼠标横向位移旋转 `moveDirection`
- 玩家角色在 `FixedUpdate()` 中持续按当前方向前进

也就是说，当前版本不是按住某个键才前进，而是角色默认持续前进，玩家只负责“修正方向”。

### 5.2 计分与胜负

`PlayerMovement` 同时处理：

- 按时间累积分数
- 掉落高度判定失败
- 分数达到阈值判定胜利
- 胜利 / 失败时停止刚体、暂停时间
- 重新开始与返回主菜单

从当前 `SampleScene` 的序列化配置来看，默认参数大致是：

- `moveSpeed = 5`
- `scorePerSecond = 1`
- `targetScore = 60`
- `deathHeight = -5`

这意味着当前默认节奏更像一段时长约 1 分钟的持续保持型跑酷训练。

### 5.3 平台生成

平台生成由 `Assets/Scripts/PlatformSpawner.cs` 负责。

它的当前实现比较直白：

- 沿着 `Z` 轴直线生成平台
- 开局一次性生成较长的初始路径
- 当队列中的平台数量低于阈值时继续补充

从场景序列化配置看，默认参数大致是：

- `startPosition = (0, 0.27, 3.9)`
- `platformSpacing = 2.7`
- `initialCount = 106`
- `threshold = 10`
- `replenishAmount = 10`

这说明当前地图设计重点不是分岔或复杂转角，而是长距离持续保持与微调。

### 5.4 平台坠落

`Assets/Scripts/FallingPlatform.cs` 会在玩家离开平台后延迟触发下落，并在掉到指定高度后销毁对象。

这给跑酷过程增加了“一路向前，不回头”的压力，同时也避免场景里旧平台无限堆积。

### 5.5 相机跟随

`Assets/Scripts/CameraFollow.cs` 负责主相机跟随。

当前场景配置更像“后上方跟随视角”而不是严格数学意义上的等长投影：

- `smoothSpeed = 0.5`
- `heightOffset = 3`
- `forwardOffset = -6`
- `lookAtPlayer = true`

所以从实际观感来说，这是一款带有等长风格命名的第三人称跟随跑酷原型。

## 6. 输入与交互说明

当前项目里真正进入主玩法链的输入只有两类：

- 鼠标横向移动：控制玩家转向
- `Esc`：暂停 / 恢复

我在脚本层扫描到的输入来源只有：

- `PlayerMovement.cs` 里的 `Input.GetAxis("Mouse X")`
- `PauseController.cs` 里的 `Input.GetKeyDown(KeyCode.Escape)`

因此可以明确判断：

- 当前版本不是键盘跑酷
- 也不是鼠标左右键跑酷
- 而是“自动前进 + 鼠标横移修正方向”

## 7. 菜单、UI 与音频系统

### MainMenuController

`Assets/Scripts/MainMenuController.cs` 负责：

- 开始游戏
- 打开设置
- 退出游戏
- 创建或复用全局 `BgmPlayer`
- 根据场景切换菜单 / 游戏 BGM

### PauseController

`Assets/Scripts/PauseController.cs` 负责：

- `Esc` 暂停
- 恢复游戏
- 返回主菜单

它是通过 `GameManager` 来统一切换暂停状态与 `Time.timeScale` 的。

### SettingsController

`Assets/Scripts/SettingsController.cs` 负责：

- 音量控制
- `PlayerPrefs` 保存用户设置
- `AudioMixer` 参数应用

## 8. 配置、日志与联调系统

### ReadConfig

`Assets/Scripts/QtMysqlUDP/ReadConfig.cs` 会在启动时读取 `config/config.ini`，并常驻到场景切换之后。

当前配置文件覆盖的内容包括：

- MySQL 连接参数
- 设备零位、传动比、分辨率
- 腕关节三轴角度范围
- Unity / Qt 的 UDP 地址与端口
- 一些训练或 ROM 相关参数

### FileLogger / ZLog

日志系统会把运行日志写入本地 `logUnity/` 目录，并按日期轮换。

这对原型调试很有帮助，因为菜单切换、暂停、游戏状态变化和设备初始化都能留痕。

### SqlAccess

`Assets/Scripts/QtMysqlUDP/SqlAccess.cs` 已经封装了：

- 连接数据库
- 查询患者侧别
- 查询训练次数
- 插入多张业务表数据

但从当前主玩法链来看，这些能力并没有被完整串入一个“可直接运行的数据上报流程”。

## 9. 编码器与硬件系统

工程里保留了完整的硬件相关目录：

- `Assets/Scripts/ScriptsUDP/EncoderManager.cs`
- `Assets/Scripts/ScriptsUDP/passiveEncoder.cs`
- `Assets/Scripts/ScriptsUDP/PCANBasic.cs`
- `Assets/Scripts/ScriptsUDP/01_LookUpChannel.cs`
- `Assets/Scripts/ScriptsUDP/03_ManualRead.cs`
- `Assets/Scripts/ScriptsUDP/04_ManualWrite.cs`

可明确读到的硬件链路信息包括：

- `PEAK-System`
- `PCAN_USB`
- `PCAN_USBBUS1`
- `Qt` / `UDP`
- `MySQL`

当前 `EncoderManager` 的职责主要是：

- 从 `ReadConfig.instance` 读取设备参数
- 调用 CAN 读写逻辑
- 把读取结果映射成自身对象的 `transform.rotation`

但要注意：

- 当前 `PlayerMovement` 并没有引用 `EncoderManager`
- 当前主玩法输入只来自鼠标横向移动
- 场景中的 `Encoder2` 更像“保留在工程里的设备对象”，不是玩家控制器的一部分

所以更准确的说法是：

当前仓库已经包含康复硬件链路，但默认可玩的 `Isometric Dash` 版本仍然是鼠标驱动的跑酷原型。

## 10. 本地运行建议

如果你只是想体验或继续改玩法层，推荐最小步骤是：

1. 用 `Unity Hub` 打开项目根目录。
2. 选择 `Unity 2022.3.53f1c1`。
3. 打开 `Assets/Scenes/Start.unity`。
4. 点击 `Play`。
5. 进入 `SampleScene` 后直接用鼠标左右移动控制方向。

如果你想继续联调外设：

1. 检查 `config/config.ini`
2. 准备 `PEAK PCAN` 驱动和设备
3. 确认 `Qt` 和 `UDP` 配置一致
4. 确认本地数据库环境可用

## 11. 当前技术债与风险

这里有几处接手时很值得先注意：

### 11.1 主玩法与硬件链尚未接通

虽然仓库里有完整的编码器 / CAN 模块，但当前玩家控制仍是鼠标驱动。  
如果后续目标是“真正由康复设备控制角色”，优先要改的不是 README，而是把硬件角度数据接进 `PlayerMovement`。

### 11.2 配置文件里仍然带着数据库连接信息

`config/config.ini` 当前直接包含数据库地址、用户名和密码。  
这对本地联调方便，但对公开仓库或多人协作并不安全，建议改成占位值或外部环境配置。

### 11.3 场景序列化字段和当前脚本存在漂移

`SampleScene.unity` 里的 `PlayerMovement` 组件仍保留了一些当前源码里已经看不到的字段，例如：

- `anglePerUnit`
- `maxSteerAngle`
- `deadzone`
- `zOffset`
- `invertZ`
- `autoRestartDelay`

而源码实际公开的字段是 `turnSensitivity`。  
这说明项目经历过脚本改名或字段重构，但场景还残留着旧序列化信息，后续在 Unity 里重新保存时可能继续暴露这类问题。

### 11.4 restartButton 当前引用为空

`SampleScene.unity` 里 `PlayerMovement.restartButton` 目前是空引用，但代码在 `Start()` 里直接调用了：

```csharp
restartButton.onClick.AddListener(RestartGame);
```

这意味着接手时最好先在 Unity 编辑器里确认该引用是否已在其他层补上，否则存在启动或场景初始化阶段的空引用风险。

## 12. 推荐阅读顺序

如果你刚接手这个项目，我建议按下面顺序看：

1. `README.md`
2. `guide.md`
3. `Assets/Scripts/PlayerMovement.cs`
4. `Assets/Scripts/PlatformSpawner.cs`
5. `Assets/Scripts/MainMenuController.cs`
6. `Assets/Scripts/PauseController.cs`
7. `Assets/Scripts/QtMysqlUDP/ReadConfig.cs`
8. `Assets/Scripts/ScriptsUDP/EncoderManager.cs`

## 13. 后续迭代建议

如果想把这个项目继续整理得更成熟，可以优先做这几件事：

- 把公开仓库里的敏感配置改成模板或占位值
- 清理 `PlayerMovement` 与场景序列化字段漂移
- 明确“纯游戏玩法模式”和“硬件联调模式”的边界
- 决定是否真的要把外设输入接管玩家运动
- 增加一份更短的研究 / 课程展示说明，方便非技术读者快速理解项目价值

## 14. 结论

`Isometric Dash` 当前已经具备一个可公开展示、可继续迭代的 Unity 康复跑酷原型基础：

- 主菜单到游戏场景的流程完整
- 鼠标主控玩法已经闭环
- 胜利 / 失败 / 暂停 / 音量设置都具备
- 外设与联调代码被完整保留

它现在最适合被理解为：

“一个已经能玩、也保留了康复系统扩展能力的 Unity 3D 康复跑酷原型工程。”
