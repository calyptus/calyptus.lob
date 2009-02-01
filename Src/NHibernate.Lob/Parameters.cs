﻿using System.Collections;
using NHibernate.Lob.Compression;
using System.Text;
using System;
using NHibernate.UserTypes;

namespace NHibernate.Lob
{
	internal static class Parameters
	{
		internal static void GetBlobSettings(IDictionary parameters, out IStreamCompressor compression)
		{
			string compr = parameters["compression"] as string;
			if (string.IsNullOrEmpty(compr))
				compression = null;
			else if (compr.Equals("gzip", StringComparison.OrdinalIgnoreCase))
				compression = GZipCompressor.Instance;
			else
			{
				System.Type compressor = System.Type.GetType(compr);
				compression = (IStreamCompressor)System.Activator.CreateInstance(compressor);
				IParameterizedType parameterized = compression as IParameterizedType;
				if (parameterized != null)
					parameterized.SetParameterValues(parameters);
			}
		}

		internal static void GetClobSettings(IDictionary parameters, out Encoding encoding, out IStreamCompressor compression)
		{
			string compr = parameters["compression"] as string;
			if (string.IsNullOrEmpty(compr))
				compression = null;
			else if (compr.Equals("gzip", StringComparison.OrdinalIgnoreCase))
				compression = GZipCompressor.Instance;
			else
			{
				System.Type compressor = System.Type.GetType(compr);
				compression = (IStreamCompressor)System.Activator.CreateInstance(compressor);
				IParameterizedType parameterized = compression as IParameterizedType;
				if (parameterized != null)
					parameterized.SetParameterValues(parameters);
			}
			
			string enc = parameters["encoding"] as string;
			if (!string.IsNullOrEmpty(enc)) encoding = Encoding.GetEncoding(enc);
			else encoding = null;
		}

		internal static void GetXlobSettings(IDictionary parameters, out IXmlCompressor compression)
		{
			string compr = parameters["compression"] as string;
			if (string.IsNullOrEmpty(compr))
				compression = null;
			else if (compr.Equals("gzip", StringComparison.OrdinalIgnoreCase))
				compression = new XmlTextCompressor(GZipCompressor.Instance);
			else
			{
				System.Type compressor = System.Type.GetType(compr);
				if (typeof(IXmlCompressor).IsAssignableFrom(compressor))
					compression = (IXmlCompressor)System.Activator.CreateInstance(compressor);
				else if (typeof(IStreamCompressor).IsAssignableFrom(compressor))
					compression = new XmlTextCompressor((IStreamCompressor)System.Activator.CreateInstance(compressor));
				else
					throw new Exception("Unknown compression type.");
			}
			IParameterizedType parameterized = compression as IParameterizedType;
			if (parameterized != null) parameterized.SetParameterValues(parameters);
		}
	}
}