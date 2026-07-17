# SHSM (Software Emulated Hardware Security Module)
SHSM is a service that isolates key operations from client processes, designed to prevent keys from being leaked in memory. It is provided via an HTTP API and only supports programming languages ​​capable of handling memory securely. It is not intended to replace hardware HSMs, but rather to provide a usable option in environments where hardware HSMs are unavailable.

## Developer's information
Upon testing SHSM with the "TestData", developer should avoid doing any operations that had **Root/root** marked with it. **Sudo/sudo** will be good to go in a production ready environment..

The client's app can also be used as a guidance to allow easier development on creating proper HTTP API calls. 

## Documentation
For details, kindly refer to either one of these links.

[Project Documentations](https://github.com/Chewhern/SHSM/tree/main/Project_Documentation)

[NLNET Documentations](https://github.com/Chewhern/SHSM/tree/main/NLNET_Documentation)

# SHSM (軟件模擬硬件安全模塊)
SHSM 是一个将密钥操作从客户端进程中隔离出来的服务，旨在防止密钥在内存中被泄露。它通过 HTTP API 提供服务，仅支持能安全处理内存的编程语言。它不是为了替代硬件 HSM，而是为了在没有硬件 HSM 的环境中提供一种可用的选项。

## 开发者须知
使用“TestData”测试 SHSM 时，开发者应避免执行任何带有 **Root/root** 标记的操作。在生产环境中，可以使用 **sudo/sudo**。

客户端应用程序也可作为参考，帮助开发者更轻松地创建正确的 HTTP API 调用。

## 文档
详情请参考以下任一链接。你可能需要用大模型去翻譯。

[項目文檔](https://github.com/Chewhern/SHSM/tree/main/Project_Documentation)

[NLNET文檔](https://github.com/Chewhern/SHSM/tree/main/NLNET_Documentation)
