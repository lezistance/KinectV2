﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace v2SwipeGesture
{
	public abstract class GestureDetector
	{
		public enum GestureType
		{
			SwipeRightToLeft = 0,
			SwipeLeftToRight = 1,
			SwipeTopToBottom = 2,
			SwipeBottomToTop = 3,
		}

		DateTime lastGestureDate = DateTime.Now;
		readonly int windowSize;
		readonly List<Entry> entries = new List<Entry>();

		public int MinimalPerBetweenGestures { get; set; }
		public event Action<GestureType> OnGestureDetected;

		protected List<Entry> Entries
		{
			get { return entries; }
		}

		public int WindowSize
		{
			get { return windowSize; }
		}

		protected GestureDetector(int windowSize = 20)
		{
			this.windowSize = windowSize;
			MinimalPerBetweenGestures = 0;
		}

		public virtual void Add(CameraSpacePoint cp, JointType jt)
		{
			Vector3 vc = new Vector3(cp.X,cp.Y,cp.Z);
			Entry newEntry = new Entry { Position = vc, Time = DateTime.Now };
			Entries.Add(newEntry);

			if (Entries.Count > WindowSize)
			{
				Entry entryToRemove = Entries[0];
				Entries.Remove(entryToRemove);
			}

			LookForGesture(jt);
		}

		protected abstract void LookForGesture(JointType jt);

		protected void RaiseGestureDetected( GestureType gType )
		{
			if (DateTime.Now.Subtract(lastGestureDate).TotalSeconds > MinimalPerBetweenGestures)
			{
				if (OnGestureDetected != null) OnGestureDetected(gType);
				lastGestureDate = DateTime.Now;
			}
			Entries.Clear();
		}
	}
}
