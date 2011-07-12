﻿using NHibernate.Lob.Compression;
using NHibernate.UserTypes;
using System;
using System.Collections;
using System.IO;
using Calyptus.Lob;
using System.Xml;
using System.Collections.Generic;

namespace NHibernate.Lob.External
{
	public class ExternalXlobType : AbstractExternalBlobType, IParameterizedType
	{
		private IXmlCompressor compression;
		public IXmlCompressor Compression
		{
			get
			{
				return compression;
			}
		}

		public ExternalXlobType()
		{
			compression = new XmlTextCompressor();
		}

		public virtual void SetParameterValues(IDictionary<string, string> parameters)
		{
			IXmlCompressor c;
			Parameters.GetXlobSettings(parameters, out c);
			if (c != null) this.compression = c;
		}

		protected override object CreateLobInstance(IExternalBlobConnection connection, byte[] identifier)
		{
			return new ExternalXlob(connection, identifier, compression);
		}

		protected override bool ExtractLobData(object lob, out IExternalBlobConnection connection, out byte[] identifier)
		{
			ExternalXlob xlob = lob as ExternalXlob;
			if (xlob == null)
			{
				connection = null;
				identifier = null;
				return false;
			}
			connection = xlob.Connection;
			identifier = xlob.Identifier;
			return true;
		}

		protected override void WriteLobTo(object lob, Stream output)
		{
			Xlob xlob = lob as Xlob;
			if (xlob == null) return;
			using (XmlWriter xw = compression.GetCompressor(output))
				xlob.WriteTo(xw);
		}

		public override System.Type ReturnedClass
		{
			get { return typeof(Xlob); }
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj) && this.compression.Equals(((ExternalXlobType)obj).compression);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
