# SHSM (Software Emulated Hardware Security Module)

## The Problem

In managed-language applications such as C#, Java, Go, Python, and Node.js, sensitive key material may be copied or retained in application memory for longer than intended. Immutable `String` values are a visible example, but the same concern can apply to `byte[]` and other representations when the runtime or surrounding libraries do not provide deterministic memory handling.

This creates a **key-material exposure window outside the HSM security boundary**. Hardware HSMs protect key material within the HSM; SoftHSM and PKCS#11 implementations provide protection within their respective software boundaries. However, key material may still exist in application memory before it reaches such a boundary.

## What SHSM Does

SHSM is a service that separates cryptographic operations from client processes and provides deliberate memory-lifecycle controls for sensitive material within the client-to-server path.

It is designed to minimize the lifetime of sensitive material in memory regions controlled by the SHSM client and server, including during key import, generation, and transmission.

* Provided through an HTTP API
* Language-independent at the API level
* Provides client/reference examples for multiple programming languages
* Focuses on the pre-HSM application layer: key import, generation, and transmission

Some potential use cases include:

* Complementing an HSM in **BYOK (Bring Your Own Key)** workflows where key material must be handled by an application before reaching the HSM boundary.
* Providing an additional memory-handling layer in environments without an HSM.
* Providing an option for applications whose existing implementation does not provide the memory-lifecycle controls required by their threat model.

## What SHSM Is Not

* Not a replacement for hardware HSMs
* Not a PKCS#11 implementation
* Not a replacement for SoftHSM
* Not a FIPS-certified module

SHSM is intended for environments where hardware HSMs are unavailable, unsuitable, or too costly, and where an additional application-layer security boundary is useful.

## Developer's Information

When testing SHSM with `TestData`, avoid operations marked `Root`/`root`.

For production deployments, administrative operations requiring elevated privileges should be performed using the appropriate operating-system privilege mechanism, such as `sudo` on Linux.

The client application can also be used as a reference for constructing HTTP API calls.

## Documentation

For further details, see the existing `*_Documentation` directories.

Documentation is currently primarily available in English. Additional translations may be provided as the project develops.

# SHSM（软件模拟硬件安全模块）

## 问题所在

在使用 C#、Java、Go、Python 和 Node.js 等托管语言编写的应用程序中，敏感密钥材料可能会被复制或在应用程序内存中保留超出预期的时间。不可变的 `String`（字符串）值就是一个明显的例子；当运行时环境或相关库无法提供确定性的内存管理机制时，`byte[]`（字节数组）及其他数据表示形式也可能面临同样的问题。

这导致了**密钥材料在 HSM 安全边界之外的暴露窗口**。硬件 HSM 在其内部保护密钥材料；SoftHSM 和 PKCS#11 实现则在其各自的软件边界内提供保护。然而，在密钥材料到达这些边界之前，它仍可能存在于应用程序内存中。

## SHSM 的功能

SHSM 是一项将加密操作与客户端进程分离的服务，它针对客户端到服务器传输路径中的敏感材料，提供了经过精心设计的内存生命周期控制机制。

其设计目标是最大限度缩短敏感材料在 SHSM 客户端和服务器所控制内存区域中的驻留时间，涵盖密钥导入、生成和传输等各个环节。

* 通过 HTTP API 提供服务
* API 层面与编程语言无关
* 提供多种编程语言的客户端示例/参考实现
* 专注于 HSM 之前的应用层环节：密钥导入、生成和传输

潜在的使用场景包括：

* 在 **BYOK（自带密钥）** 工作流中作为 HSM 的补充，即密钥材料在到达 HSM 边界前必须先由应用程序进行处理的场景。
* 在没有 HSM 的环境中提供额外的内存处理层。
* 为那些现有实现无法满足其威胁模型所要求的内存生命周期控制标准的应用程序提供一种解决方案。

## SHSM 不包含的内容

* 并非硬件 HSM 的替代品
* 并非 PKCS#11 实现
* 并非 SoftHSM 的替代品
* 并非 FIPS 认证模块

SHSM 适用于硬件 HSM 不可用、不适用或成本过高，且需要额外应用层安全边界的环境。 ## 开发者须知

在使用 `TestData` 测试 SHSM 时，请避免执行标记为 `Root` 或 `root` 的操作。

对于生产环境部署，需要提升权限的管理操作应通过相应的操作系统权限机制（例如 Linux 上的 `sudo`）来执行。

客户端应用程序也可作为构建 HTTP API 调用的参考。

## 文档

有关更多详细信息，请参阅现有的 `*_Documentation` 目录。

目前文档主要以英文提供。随着项目的发展，可能会提供其他语言版本。
