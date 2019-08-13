using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Utilities.Exceptions;
using Utilities.Logging;

namespace Utilities
{
    public static class Certificate
    {

		private const string ClientAuthenticationOID = "1.3.6.1.5.5.7.3.2";

		/// <summary>
		/// Loads a certificate as an embedded assembly resource.
		/// </summary>
		/// <param name="name">The name of the resource, without extension, it's assumed to be pfx.</param>
		/// <param name="password">The certificate's file password</param>
		/// <returns></returns>
		public static X509Certificate2 FromResourceStream(string name, string password)
        {
            Assembly assembly = typeof(Certificate).Assembly;
            string streamName = string.Format("{0}.{1}.pfx", typeof(Certificate).Namespace, name);
            Logger.Debug(typeof(Certificate), "Loading certificate " + name + " from " + streamName + "...");
            using (Stream stream = assembly.GetManifestResourceStream(streamName))
            {
                X509Certificate2 certificate = new X509Certificate2(ReadStream(stream), password, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
                Logger.Debug(typeof(Certificate), "Certificate loaded, thumb print = " + certificate.Thumbprint);
                return certificate;
            }
        }

        /// <summary>
        /// Loads a certificate as an embedded assembly resource.
        /// </summary>
        /// <param name="file">The absolute path to the file</param>
        /// <param name="password">The certificate's file password</param>
        /// <returns></returns>
        public static X509Certificate2 FromFile(string file, string password)
        {
            Logger.Debug(typeof(Certificate), "Loading certificate from file " + file + "...");
            Throw<ConfigurationException>.When(!File.Exists(file), "The certificate file " + file + " does not exist");
            using (Stream stream = File.OpenRead(file))
            {
                X509Certificate2 certificate = new X509Certificate2(ReadStream(stream), password, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
                Logger.Debug(typeof(Certificate), "Certificate loaded, thumb print = " + certificate.Thumbprint);
                return certificate;
            }
        }

		public static bool IsValidClientCertificate(X509Certificate2 cert) {
			foreach (X509Extension extension in cert.Extensions) {
				if ((extension is X509EnhancedKeyUsageExtension eku) && !IsValidForClientAuthenticationEKU(eku)) {
					return false;
				} else if ((extension is X509KeyUsageExtension ku) && !IsValidForDigitalSignatureUsage(ku)) {
					return false;
				}
			}

			return true;
		}

		public static bool IsValidForClientAuthenticationEKU(X509EnhancedKeyUsageExtension eku) {
			foreach (var oid in eku.EnhancedKeyUsages) {
				if (oid.Value == ClientAuthenticationOID) {
					return true;
				}
			}

			return false;
		}

		public static bool IsValidForDigitalSignatureUsage(X509KeyUsageExtension ku) {
			const X509KeyUsageFlags RequiredUsages = X509KeyUsageFlags.DigitalSignature;
			return (ku.KeyUsages & RequiredUsages) == RequiredUsages;
		}

		private static byte[] ReadStream(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}