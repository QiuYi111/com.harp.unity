# Harp Unity 插件包 (com.harp.unity)

这是一个为 Unity 开发的极简 Harp 设备通信插件包。它通过剥离 `Bonsai.Core` 的复杂依赖，将 `Harp.Behavior` 和 `Bonsai.Harp` 的核心功能整合进一个标准的 Unity Package Manager 包中。

## 主要特性

- **轻量化**：去除了所有 Bonsai 工作流组件和编辑器依赖，仅保留核心通信逻辑。
- **即插即用**：符合 UPM 规范，支持通过 `manifest.json` 本地引用。
- **性能优化**：直接集成 .NET Standard 2.0 兼容的 `System.IO.Ports` 和 `System.Reactive` 库，确保在 Unity Editor 和 Standalone 中稳定运行。
- **支持 Harp Behavior**：内置对 Harp Behavior 设备 (WhoAmI: 1216) 的完整寄存器支持。

## 安装方法

1. 将 `com.harp.unity` 文件夹放置在你的项目目录外（例如 `third_party` 文件夹）。
2. 打开 Unity 项目的 `Packages/manifest.json` 文件。
3. 在 `dependencies` 节点中添加以下引用：
   ```json
   "com.harp.unity": "file:../../trird_party/com.harp.unity"
   ```

   *(路径请根据你的实际目录结构调整)*

## 使用示例

### 异步写入示例（如：给予奖励）

```csharp
using Harp.Behavior;
using Bonsai.Harp;

public async void GiveReward()
{
    // 假设 _harp 是已经初始化的 Device 实例
    await _harp.WriteOutputSetAsync(DigitalOutputs.DOPort0);
}
```

### 反应式读取示例

```csharp
_harp.DeviceMessages.Subscribe(message => {
    if (message.Address == DigitalInputState.Address)
    {
        var inputState = DigitalInputState.GetPayload(message);
        Debug.Log($"输入状态更新: {inputState}");
    }
});
```

## 疑难解答

### 1. Serial Port 平台不支持错误

如果你在控制台看到 `System.IO.Ports is currently only supported on Windows`，请确保：

- 插件包中的 `System.IO.Ports.dll.meta` 文件已启用 "Editor" 平台。
- 重启 Unity 以清除旧的程序集缓存。

### 2. 编译冲突

如果出现重复类冲突，请检查并删除 `Assets/Plugins` 下旧的 Harp/Bonsai 相关 DLL 文件。本插件包已内建所有必要依赖。

## 目录结构

- `Runtime/`：核心源码和 DLL 依赖。
- `Runtime/Harp.Unity.asmdef`：Unity 程序集定义，启用了 `Unsafe` 代码支持。
- `Runtime/BonsaiTypes.cs`：用于兼容 Harp 源码的最小化 Stub 类型。

## 贡献与支持

本包旨在解决特定项目中的“DLL Hell”和版本兼容性问题。如需扩展支持其他 Harp 设备，请在 `Bonsai.Harp` 命名空间下添加相应的寄存器定义。
