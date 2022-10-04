﻿using System.Collections.Generic;
using NUnit.Framework;
using ViewObjects;
using ViewObjects.Converter;
using Cat = ViewTests.ViewTestCategories;

namespace ViewTests.Objects
{
	[TestFixture]
	[Category(Cat.UNITS)]
	public class ConversionTests
	{

		[OneTimeSetUp]
		public void Setup()
		{
			_converter = new ViewObjectsConverter();
		}

		[OneTimeTearDown]
		public void BreakDown()
		{ }

		ViewObjectsConverter _converter;

		[Test]
		public void Convert_Study()
		{
			var objs = new List<IViewObject>
			{
				new ContentReference(new List<string>
					                     { "256ff84cf7" }, ContentType.Proposed),
				new CloudReference(new List<string> { "256ff84cf7" }),
				new Viewer()
			};

			var obj = new ViewStudy(objs, "Test View Study");

			var converter = new ViewObjectsConverter();
			var res = converter.ConvertToSpeckle(obj);

			Assert.IsNotNull(res);
			Assert.IsTrue(res is ViewObjects.Speckle.ViewStudy);

			var studyBase = res as ViewObjects.Speckle.ViewStudy;
			Assert.IsTrue(obj.ViewId.Equals(studyBase.ViewId));
			Assert.IsTrue(obj.ViewName.Equals(studyBase.ViewName));
			Assert.IsTrue(obj.Objects.Count.Equals(studyBase.Objects.Count));
		}
	}
}