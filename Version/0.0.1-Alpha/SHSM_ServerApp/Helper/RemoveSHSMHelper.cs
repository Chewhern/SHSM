using ASodium;

namespace SHSM_ServerApp.Helper
{
    public static class RemoveSHSMHelper
    {
        public static void RemoveAndShift(string key)
        {
            if (!RegisteredUsersHelper.usersindices.TryGetValue(key, out int removedIndex))
                return;

            // Remove from both dictionaries
            RegisteredUsersHelper.users.Remove(key);
            RegisteredUsersHelper.usersindices.Remove(key);

            // Shift remaining indices
            var keys = RegisteredUsersHelper.usersindices.Keys.ToList(); // avoid modifying during iteration

            foreach (var k in keys)
            {
                if (RegisteredUsersHelper.usersindices[k] > removedIndex)
                {
                    RegisteredUsersHelper.usersindices[k]--;
                }
            }
        }

        public static void WholeRemoveSHSM(int SpecificIndex,String User_ID) 
        {
            if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption != IntPtr.Zero ||
                SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED255119SecretKey != IntPtr.Zero ||
                SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED448SecretKey != null ||
                SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED25519DigitalSignatureKP.CheckIsInvalid() == false ||
                SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED448DigitalSignatureRKP != null ||
                SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts != null ||
                SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].EncryptionKEMKPSecretKey != IntPtr.Zero)
            {
                if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM != null)
                {
                    if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.MACX25519KeyPair.CheckIsInvalid()==false) 
                    {
                        SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.MACX25519KeyPair.Clear();
                    }
                    if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.EncryptionX25519KeyPair.CheckIsInvalid()==false) 
                    {
                        SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.EncryptionX25519KeyPair.Clear();
                    }
                    if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.MyKEMKeyPair.CheckIsInvalid()==false) 
                    {
                        SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.MyKEMKeyPair.Clear();
                    }
                }
                if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption != IntPtr.Zero)
                {
                    SodiumGuardedHeapAllocation.Sodium_MProtect_ReadWrite(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                    SodiumGuardedHeapAllocation.Sodium_Free(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                }
                if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED255119SecretKey != IntPtr.Zero)
                {
                    SodiumGuardedHeapAllocation.Sodium_MProtect_ReadWrite(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED255119SecretKey);
                    SodiumGuardedHeapAllocation.Sodium_Free(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED255119SecretKey);
                }
                if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED448SecretKey != null)
                {
                    SodiumSecureMemory.SecureClearBytes(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED448SecretKey);
                }
                if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED25519DigitalSignatureKP != null && SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED25519DigitalSignatureKP.CheckIsInvalid()==false)
                {
                    SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED25519DigitalSignatureKP.Clear();
                }
                if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED448DigitalSignatureRKP != null)
                {
                    SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED448DigitalSignatureRKP.Clear();
                }
                if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts != null)
                {
                    SodiumSecureMemory.SecureClearBytes(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.D);
                    SodiumSecureMemory.SecureClearBytes(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.DP);
                    SodiumSecureMemory.SecureClearBytes(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.DQ);
                    SodiumSecureMemory.SecureClearBytes(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.P);
                    SodiumSecureMemory.SecureClearBytes(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.Q);
                    SodiumSecureMemory.SecureClearBytes(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.Modulus);
                    SodiumSecureMemory.SecureClearBytes(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.Exponent);
                }
                if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].EncryptionKEMKPSecretKey != IntPtr.Zero)
                {
                    SodiumGuardedHeapAllocation.Sodium_MProtect_ReadWrite(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].EncryptionKEMKPSecretKey);
                    SodiumGuardedHeapAllocation.Sodium_Free(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].EncryptionKEMKPSecretKey);
                }
                RemoveAndShift(User_ID);
                SHSMOpsHelper.ListsOfRegisteredUsers.RemoveAt(SpecificIndex);
            }
        }

        public static void PartialRemoveSHSM(int SpecificIndex, String User_ID)
        {
            if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM != null)
            {
                if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.MACX25519KeyPair.CheckIsInvalid() == false)
                {
                    SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.MACX25519KeyPair.Clear();
                }
                if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.EncryptionX25519KeyPair.CheckIsInvalid() == false)
                {
                    SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.EncryptionX25519KeyPair.Clear();
                }
                if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.MyKEMKeyPair.CheckIsInvalid() == false)
                {
                    SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].TempServerSHSM.MyKEMKeyPair.Clear();
                }
            }
            if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption != IntPtr.Zero)
            {
                SodiumGuardedHeapAllocation.Sodium_MProtect_ReadWrite(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
                SodiumGuardedHeapAllocation.Sodium_Free(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].SecretKeyForEncryption);
            }
            if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED255119SecretKey != IntPtr.Zero)
            {
                SodiumGuardedHeapAllocation.Sodium_MProtect_ReadWrite(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED255119SecretKey);
                SodiumGuardedHeapAllocation.Sodium_Free(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED255119SecretKey);
            }
            if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED448SecretKey != null)
            {
                SodiumSecureMemory.SecureClearBytes(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED448SecretKey);
            }
            if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED25519DigitalSignatureKP!=null && SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED25519DigitalSignatureKP.CheckIsInvalid()==false)
            {
                SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED25519DigitalSignatureKP.Clear();
            }
            if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED448DigitalSignatureRKP != null)
            {
                SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].ED448DigitalSignatureRKP.Clear();
            }
            if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts != null)
            {
                SodiumSecureMemory.SecureClearBytes(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.D);
                SodiumSecureMemory.SecureClearBytes(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.DP);
                SodiumSecureMemory.SecureClearBytes(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.DQ);
                SodiumSecureMemory.SecureClearBytes(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.P);
                SodiumSecureMemory.SecureClearBytes(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.Q);
                SodiumSecureMemory.SecureClearBytes(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.Modulus);
                SodiumSecureMemory.SecureClearBytes(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].RSAKeyParts.Exponent);
            }
            if (SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].EncryptionKEMKPSecretKey != IntPtr.Zero)
            {
                SodiumGuardedHeapAllocation.Sodium_MProtect_ReadWrite(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].EncryptionKEMKPSecretKey);
                SodiumGuardedHeapAllocation.Sodium_Free(SHSMOpsHelper.ListsOfRegisteredUsers[SpecificIndex].EncryptionKEMKPSecretKey);
            }
            RemoveAndShift(User_ID);
            SHSMOpsHelper.ListsOfRegisteredUsers.RemoveAt(SpecificIndex);
        }
    }
}
