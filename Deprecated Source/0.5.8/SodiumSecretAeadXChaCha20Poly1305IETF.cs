﻿using System;
using System.Security.Cryptography;

namespace ASodium
{
    public static class SodiumSecretAeadXChaCha20Poly1305IETF
    {
        public static int GetKeyLength()
        {
            return SodiumSecretAeadXChaCha20Poly1305IETFLibrary.crypto_aead_xchacha20poly1305_ietf_keybytes();
        }

        public static int GetNoncePublicLength()
        {
            return SodiumSecretAeadXChaCha20Poly1305IETFLibrary.crypto_aead_xchacha20poly1305_ietf_npubbytes();
        }

        public static int GetNonceSecurityLength()
        {
            return SodiumSecretAeadXChaCha20Poly1305IETFLibrary.crypto_aead_xchacha20poly1305_ietf_nsecbytes();
        }

        public static int GetABytesLength()
        {
            return SodiumSecretAeadXChaCha20Poly1305IETFLibrary.crypto_aead_xchacha20poly1305_ietf_abytes();
        }

        public static long GetMessageMaxLength()
        {
            return SodiumSecretAeadXChaCha20Poly1305IETFLibrary.crypto_aead_xchacha20poly1305_ietf_messagebytes_max();
        }

        public static Byte[] GeneratePublicNonce()
        {
            return SodiumRNG.GetRandomBytes(GetNoncePublicLength());
        }

        public static Byte[] GenerateKey()
        {
            Byte[] Key = new Byte[GetKeyLength()];
            SodiumSecretAeadXChaCha20Poly1305IETFLibrary.crypto_aead_xchacha20poly1305_ietf_keygen(Key);
            return Key;
        }

        public static Byte[] Encrypt(Byte[] Message, Byte[] NoncePublic, Byte[] Key, Byte[] AdditionalData = null, Byte[] NonceSecurity = null, Boolean ClearKey = false)
        {
            Byte[] CipherText = new Byte[Message.LongLength + GetABytesLength()];
            long CipherTextLength = 0;
            long MessageLength = Message.LongLength;
            long AdditionalDataLength = 0;
            if (Key == null || Key.Length != GetKeyLength())
                throw new ArgumentException("Error: Key must be " + GetKeyLength() + " bytes in length");
            if (NoncePublic == null || NoncePublic.Length != GetNoncePublicLength())
                throw new ArgumentException("Error: Public nonce must be " + GetNoncePublicLength() + " bytes in length");
            if (AdditionalData != null && (AdditionalData.Length > GetABytesLength() || AdditionalData.Length < 0))
                throw new ArgumentException("Error: Additional data must be between 0 and " + GetABytesLength() + " in bytes in length");
            if (NonceSecurity != null)
            {
                if (NonceSecurity.Length != GetNonceSecurityLength())
                {
                    throw new ArgumentException("Error: Nonce Security must exactly be " + GetNonceSecurityLength().ToString() + " bytes in length");
                }
            }
            if (AdditionalData != null && AdditionalData.Length != 0)
            {
                AdditionalDataLength = AdditionalData.LongLength;
            }
            int result = SodiumSecretAeadXChaCha20Poly1305IETFLibrary.crypto_aead_xchacha20poly1305_ietf_encrypt(CipherText, CipherTextLength, Message, MessageLength, AdditionalData, AdditionalDataLength, NonceSecurity, NoncePublic, Key);

            if (result != 0)
            {
                throw new CryptographicException("Error encrypting message.");
            }

            if (ClearKey == true)
            {
                SodiumSecureMemory.SecureClearBytes(Key);
            }

            return CipherText;
        }

        public static Byte[] Decrypt(Byte[] CipherText, Byte[] NoncePublic, Byte[] Key, Byte[] AdditionalData = null, Byte[] NonceSecurity = null, Boolean ClearKey = false)
        {
            Byte[] MessageByte = new Byte[CipherText.LongLength - GetABytesLength()];
            long MessageLength = 0;
            long CipherTextLength = CipherText.LongLength;
            long AdditionalDataLength = 0;
            if (Key == null || Key.Length != GetKeyLength())
                throw new ArgumentException("Error: Key must be " + GetKeyLength() + " bytes in length");
            if (NoncePublic == null || NoncePublic.Length != GetNoncePublicLength())
                throw new ArgumentException("Error: Public nonce must be " + GetNoncePublicLength() + " bytes in length");
            if (AdditionalData != null && (AdditionalData.Length > GetABytesLength() || AdditionalData.Length < 0))
                throw new ArgumentException("Error: Additional data must be between 0 and " + GetABytesLength() + " in bytes in length");
            if (NonceSecurity != null)
            {
                if (NonceSecurity.Length != GetNonceSecurityLength())
                {
                    throw new ArgumentException("Error: Nonce Security must exactly be " + GetNonceSecurityLength().ToString() + " bytes in length");
                }
            }
            if (AdditionalData != null && AdditionalData.Length != 0)
            {
                AdditionalDataLength = AdditionalData.LongLength;
            }

            int result = SodiumSecretAeadXChaCha20Poly1305IETFLibrary.crypto_aead_xchacha20poly1305_ietf_decrypt(MessageByte, MessageLength, NonceSecurity, CipherText, CipherTextLength, AdditionalData, AdditionalDataLength, NoncePublic, Key);

            if (result == -1)
            {
                throw new CryptographicException("Error: Verification of MAC stored in cipher text failed");
            }

            if (ClearKey == true)
            {
                SodiumSecureMemory.SecureClearBytes(Key);
            }

            return MessageByte;
        }

        public static DetachedBox CreateDetachedBox(Byte[] Message, Byte[] NoncePublic, Byte[] Key, Byte[] NonceSecurity = null, Byte[] AdditionalData = null,Boolean ClearKey=false)
        {
            DetachedBox MyDetachedBox = new DetachedBox();
            Byte[] CipherText = new Byte[Message.LongLength];
            Byte[] MAC = new Byte[GetABytesLength()];
            long MACLength = 0;
            long AdditionalDataLength = 0;
            long MessageLength = Message.LongLength;

            if (Key == null || Key.Length != GetKeyLength())
                throw new ArgumentException("Error: Key must be " + GetKeyLength() + " bytes in length");
            if (NoncePublic == null || NoncePublic.Length != GetNoncePublicLength())
                throw new ArgumentException("Error: Public nonce must be " + GetNoncePublicLength() + " bytes in length");
            if (AdditionalData != null && (AdditionalData.Length > GetABytesLength() || AdditionalData.Length < 0))
                throw new ArgumentException("Error: Additional data must be between 0 and " + GetABytesLength() + " in bytes in length");
            if (NonceSecurity != null)
            {
                if (NonceSecurity.Length != GetNonceSecurityLength())
                {
                    throw new ArgumentException("Error: Nonce Security must exactly be " + GetNonceSecurityLength().ToString() + " bytes in length");
                }
            }
            if (AdditionalData != null && AdditionalData.Length != 0)
            {
                AdditionalDataLength = AdditionalData.LongLength;
            }

            int result = SodiumSecretAeadXChaCha20Poly1305IETFLibrary.crypto_aead_xchacha20poly1305_ietf_encrypt_detached(CipherText, MAC, MACLength, Message, MessageLength, AdditionalData, AdditionalDataLength, NonceSecurity, NoncePublic, Key);

            if (result != 0)
            {
                throw new CryptographicException("Error: Failed to create detached box");
            }

            MyDetachedBox = new DetachedBox(CipherText, MAC);

            if (ClearKey == true)
            {
                SodiumSecureMemory.SecureClearBytes(Key);
            }

            return MyDetachedBox;
        }

        public static Byte[] OpenDetachedBox(DetachedBox MyDetachedBox, Byte[] NoncePublic, Byte[] Key, Byte[] AdditionalData = null, Byte[] NonceSecurity = null, Boolean ClearKey = false)
        {
            return OpenDetachedBox(MyDetachedBox.CipherText, MyDetachedBox.Mac, NoncePublic, Key, AdditionalData, NonceSecurity,ClearKey);
        }

        public static Byte[] OpenDetachedBox(Byte[] CipherText, Byte[] MAC, Byte[] NoncePublic, Byte[] Key, Byte[] AdditionalData = null, Byte[] NonceSecurity = null,Boolean ClearKey=false)
        {
            Byte[] Message = new Byte[CipherText.LongLength];
            long CipherTextLength = CipherText.LongLength;
            long AdditionalDataLength = 0;

            if (Key == null || Key.Length != GetKeyLength())
                throw new ArgumentException("Error: Key must be " + GetKeyLength() + " bytes in length");
            if (NoncePublic == null || NoncePublic.Length != GetNoncePublicLength())
                throw new ArgumentException("Error: Public nonce must be " + GetNoncePublicLength() + " bytes in length");
            if (AdditionalData != null && (AdditionalData.Length > GetABytesLength() || AdditionalData.Length < 0))
                throw new ArgumentException("Error: Additional data must be between 0 and " + GetABytesLength() + " in bytes in length");
            if (NonceSecurity != null)
            {
                if (NonceSecurity.Length != GetNonceSecurityLength())
                {
                    throw new ArgumentException("Error: Nonce Security must exactly be " + GetNonceSecurityLength().ToString() + " bytes in length");
                }
            }
            if (AdditionalData != null && AdditionalData.Length != 0)
            {
                AdditionalDataLength = AdditionalData.LongLength;
            }

            int result = SodiumSecretAeadXChaCha20Poly1305IETFLibrary.crypto_aead_xchacha20poly1305_ietf_decrypt_detached(Message, NonceSecurity, CipherText, CipherTextLength, MAC, AdditionalData, AdditionalDataLength, NoncePublic, Key);

            if (result == -1)
            {
                throw new CryptographicException("Error: Verification of MAC stored in cipher text failed");
            }

            if (ClearKey == true)
            {
                SodiumSecureMemory.SecureClearBytes(Key);
            }

            return Message;
        }
    }
}
