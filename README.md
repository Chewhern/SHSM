# SHSM (Software Emulated Hardware Security Module)
SHSM is a service that isolates key operations from client processes, designed to prevent keys from being leaked in memory. It is provided via an HTTP API and only supports programming languages ​​capable of handling memory securely. It is not intended to replace hardware HSMs, but rather to provide a usable option in environments where hardware HSMs are unavailable.

There're attempts in trying to define what should and what shouldn't an SHSM do. 

To summarize, this project had about 8 definitions.

## SHSM Definitions
1. It is a "behaviorally constrained" service, not a "physically protected" device.

SHSM's security assumptions differ from those of hardware HSM. Hardware HSM relies on physical tamper resistance (such as FIPS 140-2 Level 3), while SHSM relies on "behavioral determinism": the same input always produces the same output, and its behavior can be fully audited through code. SHSM does not claim resistance to physical attacks, but claims its software logic is public, verifiable, and predictable.

2. The SHSM server does not store keys; it only receives keys provided at runtime.

The SHSM server (the part simulating hardware) does not store plaintext copies of keys or private keys in hard-coded or file-based formats. It only receives keys or private keys provided by the client at runtime. Memory locking and automatic memory zeroing are core mechanisms to ensure maximum security.

On the client side, the developer's goal should be to minimize the exposure of keys or private keys. Except for the private key used for login, all other sensitive data or keys should be processed in a way that only the SHSM server can decrypt.

3. It supports cross-language calls, but client language type is limited by memory safety capabilities.

SHSM's API is provided via HTTP and is language-independent. However, due to software side-channel attack considerations, it currently only supports the following client programming languages: Node.js/TypeScript, Java, Python, and Go. These languages ​​can provide C-like pointers or uint8_t* data structures to support basic memory operation needs.

4. Auditability of key operations (not currently considered)

The current version does not involve anchoring key operations to Arweave or generating tamper-proof audit logs. This feature will be evaluated in future versions. It is not considered at this stage because it mainly involves considerations of ensuring data integrity. The most economical and relatively secure option currently is to use services using Western public blockchain technologies. However, it is possible that people will use server hosting providers that can ensure end-to-end server security. The latter is often more expensive than the former, which is a major drawback in terms of cost-effectiveness. It may also be used for some government or highly confidential purposes, in which case relying on blockchain or server hosting is basically not feasible.

5. Threat Model

SHSM's threat model is clearly defined:

- No Defense: Physical access attacks, chip-level side-channel attacks, operating system kernel-level intrusions.

- Defense: Application memory leaks, cross-language key remnants, key exposure due to misconfiguration (such as exposing services to insecure networks, using weak cryptographic parameters), and software-level side-channel attacks.

Users should understand this set of boundaries before deploying SHSM.

6. License and Distribution Model

SHSM is licensed under the AGPL, allowing anyone to deploy, modify, and audit its code. It is not primarily distributed as a hosted service, but rather exists as "self-deployable software."

7. Deployment Prerequisites

The core security prerequisite for SHSM is that every device deploying the SHSM server (not the client) must be configured not to use cryptographic algorithms that require specific hardware for security (e.g., disabling AES-GCM if AES-NI support is unavailable). Following auditing standards like FIPS could trigger export bans similar to those imposed on early Cisco devices due to their use of cryptographic techniques. This clause aims to prevent the introduction of new security risks due to improper algorithm deployment. Of course, if the device has specific hardware, supporting specific cryptographic algorithms is also possible. However, the default is to not use or rely on cryptographic algorithms that require specific hardware for security.

8. This primarily provides a practically executable framework and a full-stack technology chain. It aims to provide locally filtered HSM functionality while simultaneously providing security features such as API key protection.

## Developer's information
Upon testing SHSM with the "TestData", developer should avoid doing any operations that had **Root/root** marked with it. **Sudo/sudo** will be good to go in a production ready environment..

The client's app can also be used as a guidance to allow easier development on creating proper HTTP API calls. 

## Current limitations/Bugs
1. Sometimes the project may result in sudden crashing on the server side, this's primarily due to the use of pointers or unmanaged memory coming from libsodium or C language as a whole. This did get mitigated by preventing the use of certain functions on the client's app.
2. Sometimes the project may have runtime error of **Arithmetic operations resulted in an overflow.**, I am not sure if this will get triggered again. Prior to uploading server side's code, I did changed the data copying from integer variant into long variant inside **PublicKeyCryptography** and **SecretKeyCryptography** on the server side for safety measures. If it did happen, can let me know by issuing issues.

## Documentation
There're a lot that I didn't mention here. I also do think that documentations were really required for this project. Give me some time, I will sort things out and put here..

# SHSM (軟件模擬硬件安全模塊)
SHSM 是一个将密钥操作从客户端进程中隔离出来的服务，旨在防止密钥在内存中被泄露。它通过 HTTP API 提供服务，仅支持能安全处理内存的编程语言。它不是为了替代硬件 HSM，而是为了在没有硬件 HSM 的环境中提供一种可用的选项。

有人尝试定义高中毕业考试（SHSM）应该做什么和不应该做什么。

总而言之，这个项目包含了大约 8 个定义。

## SHSM的定義
1. 它是一个“行为被约束”的服务，而不是一个“物理被保护”的设备
SHSM 的安全假设与硬件 HSM 不同。硬件 HSM 依赖物理防篡改（如 FIPS 140-2 Level 3），而 SHSM 依赖 “行为确定性”：相同输入永远产生相同输出，且其行为可以通过代码完整审计。SHSM 不宣称能抵抗物理攻击，但宣称其软件逻辑是公开、可验证且可预测的。

2. SHSM 服务端不存储密钥，只接收运行时提供的密钥
SHSM 服务端（模拟硬件的部分）不会以硬编码或文件的方式存储密钥或私钥的明文副本。它只接收来自客户端运行时提供的密钥或私钥。为确保尽可能的安全，内存锁定和自动内存清零是核心机制。
在客户端一侧，开发者的目标应是尽可能减少密钥或私钥的暴露。除登录用的私钥外，其他敏感数据或密钥应一律采用只有 SHSM 服务端才能解密的处理方式。

3. 它支持跨语言调用，但客户端语言类型受限于内存安全能力
SHSM 的 API 通过 HTTP 提供，语言无关。但由于软件侧信道攻击的考量，目前仅支持以下客户端编程语言：Node.js/TypeScript、Java、Python 和 Go。这些语言能够提供类似 C 语言的指针或 uint8_t* 类型的数据结构，以支持基本的内存操作需求。

4. 密钥操作的可审计性（暂不考虑）
当前版本暂不涉及将密钥操作锚定到 Arweave 或生成不可篡改的审计日志。该功能将在未来版本中评估。現階段沒考慮因爲主要涉及到如何確保數據不被篡改類的考量，現有的最經濟也相對安全的是使用西方的公鏈區塊鏈技術類的服務。但也有可能人們會使用那些可以確保服務器托管全程安全的服務器托管商。而後者往往比前者更加的昂貴，在性價比上會是硬傷。也有可能會被用於一些政府或者高度機密的用途上，而這樣的情況下，依賴區塊鏈或者服務器托管基本上行不通。

5. 威胁模型
SHSM 的威胁模型是明确定义的：

- 不防御：物理访问攻击、芯片级侧信道攻击、操作系统内核级入侵。

- 防御：应用程序内存泄露、跨语言密钥残留、配置错误（如将服务暴露在非安全网络、使用弱密码学参数）导致的密钥暴露，以及软件层面的侧信道攻击。

用户应在部署 SHSM 之前了解这组边界。

6. 许可证与发布模式
SHSM 是 AGPL 许可的，允许任何人自行部署、修改和审计其代码。它不以托管服务为主要分发方式，而是以“可自部署的软件”形态存在。

7. 部署前置条件
SHSM 的核心安全前提是：每个部署 SHSM 服务端的设备（而非客户端），都必须已经配置为不使用依赖特定硬件才能安全的密码学算法（如无 AES-NI 支持时禁用 AES-GCM）。若以 FIPS 等审计标准为准则，则可能触发类似早期 Cisco 因其使用密码学技术而被禁止出口的禁令。此条款旨在防止因算法部署不当引入新的安全风险。當然若該設備有特定硬件，那支持特定的密碼學算法也可行。但默認狀態下是不使用或者依賴特定硬件才能安全的密碼學算法。

8. 這主要是提供一個可實際運行的框架和全棧技術鏈。它意在提供可提供的局部篩選出來的HSM功能和同時提供API 鑰匙運行保護類的安全功能。

## 开发者须知
使用“TestData”测试 SHSM 时，开发者应避免执行任何带有 **Root/root** 标记的操作。在生产环境中，可以使用 **sudo/sudo**。

客户端应用程序也可作为参考，帮助开发者更轻松地创建正确的 HTTP API 调用。

## 当前限制/已知缺陷
1. 项目有时可能导致服务器端突然崩溃，这主要是由于使用了指针或来自 libsodium 或 C 语言的非托管内存。通过限制客户端应用程序中某些函数的使用，此问题已得到缓解。

2. 项目有时可能会出现“算术运算导致溢出”的运行时错误，我不确定此错误是否会再次出现。在上传服务器端代码之前，为了安全起见，我已将服务器端 **PublicKeyCryptography** 和 **SecretKeyCryptography** 中的数据复制方式从整数类型更改为长整型。如果出现问题，请提交 issue 通知我。

## 文档
这里还有很多我没提到的内容。我也认为这个项目确实需要文档。请给我一些时间，我会整理好并放在这里。
