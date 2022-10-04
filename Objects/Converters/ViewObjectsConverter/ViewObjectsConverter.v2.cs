﻿using System.Collections.Generic;
using System.Linq;
using Speckle.Core.Models;
using ViewObjects.References;
using VS = ViewObjects.Speckle;
using VO = ViewObjects;

namespace ViewObjects.Converter
{
	/// <inheritdoc />
	public partial class ViewObjectsConverter
	{
		IViewObject StudyToNative(VS.ViewStudy obj) => new Study.ViewStudy
		(
			obj.Objects.Valid() ? obj.Objects.Where(x => x != null).Select(ConvertToNativeViewObject).ToList()
				: new List<IViewObject>(),
			obj.ViewName,
			obj.ViewId
		);

		IViewObject ViewContentToNative(VS.Content obj) => new ContentReference(obj.References, obj.ContentType, obj.ViewId, obj.ViewName);

		IViewObject ViewCloudToNative(IReferenceObject obj) => new CloudReference(obj.References, obj.ViewId);

		IViewObject LayoutToNative(IViewerLayout obj) => new Viewer.Layout(obj.Viewers);

		IViewObject ViewerToNative(IViewer<VS.Layout> o) =>
			new Viewer.Viewer(o.Layouts.Where(x => x != null).Select(LayoutToNative).Cast<IViewerLayout>().ToList());

		IViewObject ViewerToNative(IViewerLinked<VS.Layout> o) =>
			new Viewer.ViewerLinked(o.Layouts.Where(x => x != null).Select(LayoutToNative).Cast<IViewerLayout>().ToList(), o.Clouds);

		IViewObject ResultCloudToNative(VS.ResultCloud obj) =>
			new VO.ResultCloud(obj.Points, obj.Data.Where(x => x != null).Select(ResultCloudDataToNative).ToList(), obj.ViewId);

		IResultCloudData ResultCloudDataToNative(IResultCloudData obj) => new VO.ResultCloudData(obj.Values, obj.Option, obj.Layout);

		VS.ViewStudy StudyToSpeckle(IViewStudy<IViewObject> obj) => new VS.ViewStudy
		{
			Objects = obj.Objects.Valid() ? obj.Objects.Where(x => x != null).Select(ConvertToSpeckleViewObject).ToList() : new List<VS.ViewObjectBase>(),
			ViewName = obj.ViewName,
			ViewId = obj.ViewId
		};

		VS.Content ViewContentToSpeckle(ContentReference obj) => new VS.Content(obj.ContentType, obj.References, obj.ViewId, obj.ViewName);

		VS.ViewCloud ViewCloudToSpeckle(IReferenceObject obj) => new VS.ViewCloud(obj.References, obj.ViewId);

		VS.ViewObjectReferenceBase ViewObjectReferenceToSpeckle(IReferenceObject obj) =>
			new VS.ViewObjectReferenceBase(obj.References, obj.Type, obj.ViewId, obj.ViewName);

		VS.Layout LayoutToSpeckle(IViewerLayout obj) => new VS.Layout(obj.Viewers);

		VS.Viewer ViewerToSpeckle(IViewer<IViewerLayout> o) => new VS.Viewer(
			o.Layouts.Where(x => x != null).Select(LayoutToSpeckle).ToList());

		VS.ViewerLinked ViewerToSpeckle(IViewerLinked<IViewerLayout> o) => new VS.ViewerLinked(
			o.Layouts.Where(x => x != null).Select(LayoutToSpeckle).ToList(), o.Clouds);

		VS.ResultCloud ResultCloudToSpeckle(IResultCloud obj)
		{
			return new VS.ResultCloud(obj.Points, obj.Data.Where(x => x != null).Select(ResultCloudDataToSpeckle).ToList(), obj.ViewId);
		}

		VS.ResultCloudData ResultCloudDataToSpeckle(IResultCloudData obj)
		{
			return new VS.ResultCloudData(obj.Values, obj.Option, obj.Layout);
		}

		//TODO: Support getting list of objects from search
		IViewObject HandleDefault(Base @base)
		{
			IViewObject o = default;

			if (@base.IsWrapper())
			{
				var obj = @base.SearchForType<VS.ViewObjectBase>(true);

				if (obj != null)
					o = obj;
			}

			return o;
		}

	}
}