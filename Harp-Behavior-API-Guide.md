# Harp Behavior API 使用指南

本指南详细介绍了如何使用 `Harp.Behavior` C# API 与 Harp Behavior 设备进行交互。

## 📖 概述

`Harp.Behavior` 提供了一套功能强大的接口，用于控制和采集来自 Harp Behavior 设备的数据。它支持两种主要的编程模式：
1. **异步 API (`AsyncDevice`)**: 适用于显式的请求-响应操作。
2. **响应式 API (`Device`)**: 适用于高性能的、基于流的事件驱动编程。

---

## 🚀 快速开始

### 1. 引用依赖
确保你的项目中引用了以下命名空间：
```csharp
using Harp.Behavior;
using Bonsai.Harp;
using System.Reactive.Linq; // 用于响应式编程
```

### 2. 异步模式 (Async API)
`AsyncDevice` 提供了基于 `async/await` 的方法来直接读取和写入寄存器。

#### 初始化
```csharp
// 创建异步设备实例
using var device = await Device.CreateAsync("COM3");
```

#### 读取寄存器
```csharp
// 读取数字输入状态
DigitalInputs inputs = await device.ReadDigitalInputStateAsync();

// 读取带时间戳的数据
var timestampedInputs = await device.ReadTimestampedDigitalInputStateAsync();
Console.WriteLine($"Time: {timestampedInputs.Seconds}, State: {timestampedInputs.Value}");
```

#### 写入寄存器
```csharp
// 设置数字输出
await device.WriteOutputSetAsync(DigitalOutputs.DOPort0);

// 清除数字输出
await device.WriteOutputClearAsync(DigitalOutputs.DOPort0);
```

---

### 3. 响应式模式 (Reactive API)
`Device` 类（作为 Bonsai 组合器）允许你订阅设备产生的所有消息流。

#### 设置与订阅
```csharp
var device = new Harp.Behavior.Device { PortName = "COM3" };

// 生成消息流并按地址过滤
var diStream = device.Generate()
    .Where(m => m.Address == DigitalInputState.Address && m.MessageType == MessageType.Event)
    .Select(m => DigitalInputState.GetPayload(m));

// 订阅流
using (diStream.Subscribe(inputs => {
    if (inputs.HasFlag(DigitalInputs.DIPort0)) {
        Console.WriteLine("DI Port 0 detected!");
    }
}))
{
    // 运行设备流
    // 注意：在 Bonsai 框架外可能需要额外的运行逻辑，通常配合 Reactive Extensions 使用。
}
```

---

## 📋 常用寄存器映射 (Register Map)

| 寄存器名称 | 地址 (Address) | 类型 | 说明 |
| :--- | :--- | :--- | :--- |
| `DigitalInputState` | 32 | Byte | 读取所有数字输入的状态 |
| `OutputSet` | 34 | UInt16 | 设置特定输出位为高电平 |
| `OutputClear` | 35 | UInt16 | 清除特定输出位 |
| `OutputState` | 37 | UInt16 | 读取当前所有输出的状态 |
| `AnalogData` | 44 | Int16 | 读取模拟输入数据 |
| `PwmStart` | 68 | Byte | 启动 PWM 生成 |
| `PwmStop` | 69 | Byte | 停止 PWM 生成 |
| `EncoderReset` | 108 | Byte | 重置编码器计数器 |

---

## 💡 高级示例：速度计算

参考 `SpeedTest` 项目，可以使用 Rx.NET 的 `Buffer` 和 `Scan` 操作符来实时计算脉冲频率：

```csharp
var di0RisingEdges = messageStream
    .Where(m => m.Address == DigitalInputState.Address && m.MessageType == MessageType.Event)
    .Select(m => DigitalInputState.GetPayload(m))
    .Scan(new { Current = DigitalInputs.None, Last = DigitalInputs.None }, 
          (state, next) => new { Current = next, Last = state.Current })
    .Where(s => s.Current.HasFlag(DigitalInputs.DIPort0) && !s.Last.HasFlag(DigitalInputs.DIPort0))
    .Select(_ => 1);

var speedMonitor = di0RisingEdges
    .Buffer(TimeSpan.FromSeconds(1))
    .Select(events => events.Count);

speedMonitor.Subscribe(count => Console.WriteLine($"Current Speed: {count} events/s"));
```

---

## 🔗 相关链接
- [Harp 官方文档](https://harp-tech.org/api/Harp.Behavior.html)
- [Bonsai 官网](https://bonsai-rx.org/)
