# SHSM (Software Emulated Hardware Security Module)

## The Problem

In managed languages (C#, Java, Go, Python, Node.js), key material in application memory
cannot be deterministically cleared after use. Immutable `String` is the most visible
symptom, but the issue applies to `byte[]` and other representations as well: if the
runtime or framework does not enforce zeroization, key material may persist in memory
longer than necessary.

This window lies **outside the HSM boundary**. Hardware HSMs protect keys inside the
hardware. SoftHSM and PKCS#11 libraries protect keys inside the software module. But
key material in the application layer — before it reaches any HSM boundary — is not
covered by either.

## What SHSM Does

SHSM is a service that isolates key operations from client processes, designed to
enforce memory hygiene across the client-to-server path. It clears the last in-memory
copy of sensitive material before it reaches any HSM boundary.

- Provided via an HTTP API
- Supports languages capable of handling memory securely
- Covers the pre-HSM window: key import, generation, and transmission in application memory

Some typical use cases:
- Complementary to an HSM when the client operates in **BYOK (Bring Your Own Key)** mode.
- Enforcing strict memory hygiene from client to server in environments without an HSM.
- Complementary to existing password manager services that were not coded in **C#, C, C++, or Rust**.

## What SHSM Is Not

- Not a replacement for hardware HSMs
- Not a PKCS#11 API
- Not a replacement for SoftHSM
- Not a FIPS-certified module

SHSM is intended for environments where hardware HSMs are unavailable, unsuitable, or
too expensive — and where key material still passes through application memory before
reaching any boundary.

## Developer's Information

Upon testing SHSM with "TestData", avoid any operations marked with Root/root.
Sudo/sudo is appropriate in a production-ready environment.

The client application can be used as a reference for creating proper HTTP API calls.

## Documentation

For details, refer to the existing provided **`*`_Documentation**.

# SHSM (軟件模擬硬件安全模塊)

## 問題所在

在使用托管語言（如 C#、Java、Go、Python、Node.js）編寫的程序中，應用程序內存中的密鑰材料在使用後無法被確定性地清除。
不可變的 `String`（字符串）是最明顯的例子，但 `byte[]`（字節數組）及其他數據表示形式也存在同樣的問題：
如果運行時環境或框架不強制執行內存清零（zeroization），密鑰材料可能會在內存中停留超出必要的時間。

這一風險窗口存在於 **HSM 邊界之外**。
硬件 HSM 在硬件內部保護密鑰；SoftHSM 和 PKCS#11 庫則在軟件模塊內部保護密鑰。
然而，位於應用層——即密鑰到達任何 HSM 邊界之前——的密鑰材料，並不受上述任何機制的保護。

## SHSM 的功能

SHSM 是一項將密鑰操作與客戶端進程相隔離的服務，旨在強制執行從客戶端到服務器全鏈路的內存安全規範（內存衛生）。
它會在敏感材料到達任何 HSM 邊界之前，清除其在內存中的最後一份副本。

- 通過 HTTP API 提供服務
- 支持能夠安全處理內存的編程語言
- 覆蓋 HSM 之前的處理階段：即應用程序內存中的密鑰導入、生成和傳輸過程

一些典型使用場景：
- 在客戶端採用 **BYOK（自帶密鑰）** 模式時，作為 HSM 的補充方案。
- 在沒有 HSM 的環境中，強制執行從客戶端到服務器的嚴格內存安全規範。
- 補充現有的密碼管理器服務（特別是那些非使用 **C#、C、C++ 或 Rust** 編寫的服務）。

## SHSM 不是什麼

- 不是硬件 HSM 的替代品
- 不是 PKCS#11 API
- 不是 SoftHSM 的替代品
- 不是 FIPS 認證模塊

SHSM 適用於無法使用、不適用或成本過高的硬件 HSM 的環境，且密鑰材料在到達任何邊界之前必須經過應用程序內存的場景。

## 開發者須知

在利用「TestData」測試 SHSM 時，請避免執行任何標記為 Root/root 的操作。在生產環境中，使用 sudo 是合適的。

客戶端應用程序可作為參考，用於構建正確的 HTTP API 調用。

## 文檔

有關詳細信息，請參閱現有的 **`*`_Documentation**。

目前以英文為主。如有需要，請聯絡我，我會提供相應的翻譯文檔。
