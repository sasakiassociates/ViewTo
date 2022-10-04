﻿using System.Collections.Generic;
using System.Linq;
using ViewObjects;

namespace ViewTo.Cmd
{
	internal class InitializeAndBuildRig : ICmdWithArgs<SimpleResultArgs>
	{
		IRig _rig;
		IReadOnlyList<IContent> _contents;
		IReadOnlyList<IViewCloud> _clouds;
		IReadOnlyList<IViewer> _viewers;

		public SimpleResultArgs args { get; private set; }

		public InitializeAndBuildRig(IRig rig, IReadOnlyList<IContent> contents, IReadOnlyList<IViewCloud> clouds, IReadOnlyList<IViewer> viewers)
		{
			_rig = rig;
			_contents = contents;
			_clouds = clouds;
			_viewers = viewers;
		}

		public void Execute()
		{
			if (_rig == default)
			{
				args = new SimpleResultArgs(false, "Rig is not valid for initializing");
				return;
			}

			var rigParams = new List<RigParameters>();

			var layouts = new List<IViewerLayout>();
			foreach (var v in _viewers)
			{
				// if its not a global object
				if (v is IViewerLinked vl)
				{
					rigParams.Add(CreateRigParams(vl.Layouts, _contents, vl.Clouds));
				}
				else
				{
					layouts.AddRange(v.Layouts);
				}
			}

			rigParams.Insert(0, CreateRigParams(layouts, _contents, _clouds));
			_rig.Initialize(rigParams);
			_rig.Build();
		}

		/// <summary>
		/// Goes through all the viewers and places
		/// <para>this is where the view colors that are not meant to be shared globally would be separated, but for now we use all the colors</para>
		/// </summary>
		/// <param name="viewers"></param>
		/// <param name="contents"></param>
		/// <param name="clouds"></param>
		public RigParameters CreateRigParams(
			IEnumerable<IViewerLayout> viewers,
			IEnumerable<IContent> contents,
			IEnumerable<IId> clouds
		)
		{
			return CreateRigParams(viewers, contents, clouds.Where(x => x != default).Select(x => x.ViewId).ToList());
		}

		/// <summary>
		/// Goes through all the viewers and places
		/// <para>this is where the view colors that are not meant to be shared globally would be separated, but for now we use all the colors</para>
		/// </summary>
		/// <param name="viewers"></param>
		/// <param name="contents"></param>
		/// <param name="clouds"></param>
		public RigParameters CreateRigParams(
			IEnumerable<IViewerLayout> viewers,
			IEnumerable<IContent> contents,
			IEnumerable<string> clouds
		)
		{
			return new RigParameters(
				clouds.ToList(),
				contents.Where(x => x != null && x.ContentType == ContentType.Target).Select(x => x.Color).ToList(),
				viewers.ToList()
			);
		}

	}
}