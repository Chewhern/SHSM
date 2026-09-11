# Key Material Handling and Client-Side Security Considerations

## Purpose

SHSM treats cryptographic security as an end-to-end engineering concern
rather than only a property of the cryptographic implementation itself.

Established cryptographic libraries, PKI systems, secret-management
systems, and HSM interfaces can provide strong protection once key
material reaches their protected component. However, sensitive material
may already have been represented, copied, serialized, encoded, or
retained by an application, SDK, CLI, or runtime before that boundary is
reached.

This document describes the security consideration behind SHSM's
approach to key-material handling across the server, client application,
and CLI.

It does **not** claim that SHSM eliminates all copies of sensitive data,
makes arbitrary consuming applications memory-safe, or provides a formal
security audit.

## 1. Immutable Data Types and Key Material

The difficulty of securely destroying sensitive data represented by
immutable strings is not a new problem.

Many programming environments provide immutable string types and
garbage-collected memory. Once sensitive material has been represented
as a string, an application may not have deterministic control over:

-   how long that representation remains in memory;
-   whether another copy has been created;
-   when the runtime reclaims the object;
-   whether intermediate strings were created during parsing or
    encoding;
-   whether serialization or logging created additional representations.

This issue becomes particularly relevant when cryptographic material is
represented using textual encodings such as Base64, PEM, JSON strings,
or hexadecimal strings.

Base64 itself is not a vulnerability. The security concern arises when
sensitive material is converted into an immutable or otherwise
difficult-to-control representation and subsequently passed through
application, SDK, CLI, serialization, or runtime layers.

For example:

``` text
Private-key bytes
    |
    v
Base64 encoding
    |
    v
Immutable string
    |
    +--> CLI argument
    +--> JSON representation
    +--> SDK object
    +--> logging/error path
```

The resulting string may remain in memory independently of the original
binary representation.

## 2. The Security Boundary Includes SDKs and CLIs

A cryptographic service can have strong internal key protection while
its client interface provides substantially weaker handling of sensitive
input.

This distinction is important for operations such as key import.

For example, an interface conceptually similar to:

``` text
import-rsa-key "<base64-private-key>"
```

requires the private key to exist as a command-line string before the
cryptographic service can process it.

Even if the receiving service subsequently places the decoded key into
protected memory and securely zeroizes it, the original command-line
representation may already have existed outside that protected boundary.

The same consideration applies to SDKs:

``` text
Application
    |
    v
SDK
    |
    v
serialization / encoding
    |
    v
HTTP/API
    |
    v
SHSM
```

The security mechanisms inside SHSM cannot retroactively erase
representations that were created inside the consuming application.

Therefore, SHSM considers the design of its CLI and client application
to be part of the security engineering problem rather than treating them
as purely convenience interfaces.

## 3. "Works" and "Security-Engineered" Are Different Properties

A cryptographic API can be functionally correct without providing strong
lifecycle control over sensitive data.

For example:

``` csharp
byte[] key = GetKey();
var result = CryptographicOperation(key);
```

may work correctly.

However, security-sensitive software may additionally need to consider:

-   how the key was obtained;
-   whether the key was copied;
-   whether the memory can be explicitly cleared;
-   how long the key remains allocated;
-   whether the key is retained by another object;
-   whether serialization created another copy;
-   whether error handling or logging exposed it;
-   whether the operating environment can expose the process memory.

SHSM therefore aims to make sensitive-memory handling an explicit
engineering concern.

This does not mean that every byte in the complete application ecosystem
can be controlled by SHSM. It means that the SHSM components themselves
should not unnecessarily rely on ordinary runtime behavior when more
deliberate mechanisms are available.

## 4. Mutable Binary Data Is an Enabling Mechanism, Not a Complete Solution

Mutable binary containers such as C# `byte[]`, C/C++ byte buffers, Go
`[]byte`, or Node.js `Buffer` provide an important capability: their
contents can be explicitly overwritten.

However:

``` text
mutable memory != automatically secure memory
```

A mutable buffer does not by itself guarantee that:

-   no copy was made;
-   a library did not retain another copy;
-   serialization did not create another representation;
-   the runtime did not relocate or duplicate data;
-   the caller actually zeroized the buffer;
-   the operating system cannot expose the memory.

Consequently, SHSM does not treat the use of `byte[]` alone as
sufficient security engineering.

The goal is instead to combine appropriate data representations with
explicit lifecycle handling where practical.

## 5. SHSM Server-Side Handling

The SHSM server uses libsodium secure-memory facilities for
security-sensitive memory handling.

The current implementation includes mechanisms such as:

-   `sodium_malloc`;
-   `sodium_mlock`;
-   `sodium_munlock`;
-   `sodium_mprotect_readonly`;
-   `sodium_mprotect_readwrite`;
-   `sodium_mprotect_noaccess`;
-   explicit secure zeroization; and
-   cleanup of sensitive allocations.

These mechanisms are used to provide deliberate control over selected
sensitive memory regions.

They should not be interpreted as a guarantee that every transient
representation created by the .NET runtime, HTTP stack, serialization
layer, operating system, or cryptographic dependency is protected in
exactly the same way.

The intended security property is narrower and more defensible: where
SHSM controls sensitive long-lived key material, it attempts to use
explicit secure-memory mechanisms instead of relying solely on normal
managed-memory lifetime and garbage collection.

## 6. SHSM Client Application and CLI

The SHSM client application and CLI may need to handle sensitive binary
material locally for operations such as authentication, key generation,
or other workflows.

Where sensitive binary material must be represented locally, the
implementation uses mutable binary data and explicit cleanup rather than
relying on immutable strings as the primary representation of sensitive
binary material.

This is particularly important for future CLI refinement.

The CLI should avoid unnecessarily requiring sensitive key material to
appear directly in:

-   process command-line arguments;
-   environment variables;
-   logs;
-   exception messages;
-   diagnostic output; or
-   other persistent textual representations.

Where an operation requires sensitive input, safer mechanisms such as
files with appropriate permissions, standard input, or other controlled
input mechanisms may be preferable depending on the workflow and threat
model.

The exact mechanism should be selected per command rather than assuming
that every secret can safely be passed as a normal command-line
argument.

**```As of now, the CLI did not require the importing of keys via command line stated in "2.". The cryptographic keys were stored in files or retrieved from files as binary streams of data which is mutable data type by default.```**

## 7. Relationship to PKI, Secret-Management, and HSM Projects

SHSM does not claim that existing PKI, secret-management, or HSM
projects are unaware of secure key handling.

Established projects can provide strong security mechanisms, including:

-   protected key stores;
-   HSM-backed private keys;
-   KMS integration;
-   wrapped-key import;
-   non-extractable key attributes;
-   secure transport;
-   memory-clearing facilities;
-   platform-specific protected storage; and
-   dedicated cryptographic processes.

The distinction being investigated by SHSM is the **scope of the
security boundary**.

A project may strongly protect key material after it reaches its
cryptographic service while leaving the calling application's
representation and lifetime management primarily to the application
developer.

This is particularly relevant to:

-   SDKs;
-   CLIs;
-   key-import workflows;
-   Base64/PEM/JSON representations;
-   language runtimes with immutable strings; and
-   applications that do not deliberately zeroize sensitive buffers.

SHSM's design attempts to make secure handling part of the service
architecture and its reference client tooling rather than assuming that
every consuming application will implement equivalent memory-handling
measures independently.

## 8. Bouncy Castle and Other Cryptographic Libraries

The use of mutable byte arrays by a cryptographic library is generally
preferable to requiring sensitive binary material to remain in immutable
strings when explicit clearing is required.

However, a mutable `byte[]` does not automatically provide secure
zeroization.

An application using a cryptographic library remains responsible for
understanding the library's ownership, copying, object-lifetime, and
cleanup behavior.

The same principle applies to SHSM's own dependencies.

SHSM therefore does not claim that using a particular programming
language, library, or data type automatically solves memory-security
problems.

Instead, the project attempts to apply additional controls where it has
sufficient control over the relevant memory and lifecycle.

## 9. SHSM's Practical Security Position

The intended position of SHSM is therefore:

> SHSM does not claim to invent secure-memory techniques or to eliminate
> the established limitations of immutable and garbage-collected data
> types. Its security engineering objective is to apply established
> secure-memory and lifecycle-control techniques to a software-HSM
> architecture, while also treating the client and CLI boundary as part
> of the key-material handling problem.

This results in several practical principles:

1.  **Do not treat immutable strings as an appropriate long-lived
    representation of sensitive binary key material when deterministic
    clearing is required.**
2.  **Do not assume that mutable byte arrays are automatically secure.**
3.  **Do not assume that protection inside the SHSM server automatically
    protects copies created by the calling application.**
4.  **Treat SDK and CLI input/output design as part of the security
    boundary.**
5.  **Prefer controlled binary representations and explicit cleanup
    where SHSM has control over the memory.**
6.  **Avoid unnecessary exposure of sensitive material through
    command-line arguments, logs, environment variables, and textual
    serialization.**
7.  **Document the remaining limits rather than claiming complete memory
    safety.**

## 10. Security Boundary and Limitations

The SHSM architecture can reduce the amount of long-lived raw key
material that a consuming application needs to manage, but it cannot
make an arbitrary consuming application memory-safe.

For example:

``` text
Application
    |
    | raw private key exists here
    v
SDK / CLI
    |
    | copies / serialization may occur
    v
HTTP / ETLS
    |
    v
SHSM process
    |
    +-- protected memory
    +-- memory locking
    +-- memory permission control
    +-- zeroization
    |
    v
Cryptographic operation
```

The protected SHSM process therefore represents a deliberate security
boundary, but the security of the complete system still depends on the
deployment environment and on how the consuming application handles
sensitive information before it reaches SHSM.

This is consistent with SHSM's broader project positioning: it is a
software-emulated HSM and is not presented as equivalent to a certified
physical HSM or as providing a universal guarantee against process,
kernel, filesystem, or physical-memory compromise.

## 11. Relationship to SHSM Development Work

The security considerations described here directly relate to ongoing
SHSM development work, particularly:

-   CLI command design;
-   safe handling of sensitive parameters and output;
-   API/CLI consistency;
-   interoperability testing;
-   key lifecycle testing;
-   regression testing;
-   deployment security documentation; and
-   implementation refinement.

The purpose of this work is not to claim a formal security audit or
cryptographic certification.

Instead, the objective is to make the existing security-sensitive
implementation more deliberate, reproducible, understandable, and
maintainable.

## 12. Scope of the Claim

SHSM should therefore avoid claims such as:

-   "C#/C++/C completely solves immutable-string security."
-   "Base64 is itself a vulnerability."
-   "Bouncy Castle does not provide secure memory handling."
-   "Existing HSMs do not protect imported keys."
-   "Other PKI projects do not care about memory security."
-   "SHSM guarantees that no copy of a secret ever exists."
-   "SHSM makes every consuming language memory-safe."

The defensible claim is narrower:

> **SHSM explicitly considers sensitive key-material handling across the
> service, client, and CLI boundaries and attempts to apply deliberate
> memory-lifecycle controls where the project has control over the
> relevant data.**

This distinction between functional correctness and security-oriented
engineering is a central consideration in the design of SHSM.
