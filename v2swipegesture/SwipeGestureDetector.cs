using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace v2SwipeGesture
{
	public class SwipeGestureDetector : GestureDetector
    {
		public float SwipeMinimalLength { get; set; }
		public float SwipeMaximalHeight { get; set; }

		public int SwipeMininalDuration { get; set; }
		public int SwipeMaximalDuration { get; set; }

		public SwipeGestureDetector( int windowSize = 20
			, float minLen = 0.4f
			, float maxHgt = 0.15f
			, int minDur = 250
			, int maxDur = 700
			)
			: base(windowSize)
		{
			SwipeMinimalLength = minLen;		//始点と終点の横幅が30cm以内
			SwipeMaximalHeight = maxHgt;     //始点と終点の高低差が15cm以内

			SwipeMininalDuration = minDur;     //ジェスチャ判定時間最小値(ミリ秒)
			SwipeMaximalDuration = maxDur;     //ジェスチャ判定時間最大値(ミリ秒)
		}

		protected bool ScanPositions(Func<Vector3, Vector3, bool> hFunc, Func<Vector3, Vector3, bool> directionFunc,
										Func<Vector3, Vector3, bool> lFunc, int minTime, int maxTime)
		{
			int start = 0;

			for (int idx = 1; idx < Entries.Count - 1; idx++)
			{
				if (!hFunc(Entries[0].Position, Entries[idx].Position) || !directionFunc(Entries[idx].Position, Entries[idx + 1].Position))
				{
					start = idx;
				}

				if (lFunc(Entries[idx].Position, Entries[start].Position))
				{
					double totalMilliseconds = (Entries[idx].Time - Entries[start].Time).TotalMilliseconds;
					if (totalMilliseconds >= minTime && totalMilliseconds <= maxTime)
					{
						return true;
					}
				}
			}			
			return false;
		}

		protected override void LookForGesture(JointType jt)
		{
			//Right to Left 
			if (ScanPositions((p1, p2) => Math.Abs(p2.Y - p1.Y) < SwipeMaximalHeight,	// Height
					(p1, p2) => p2.X - p1.X < 0.01f,									// Progression to right
					(p1, p2) => (p2.X - p1.X) > SwipeMinimalLength,						// Length
					SwipeMininalDuration, SwipeMaximalDuration))						// Duration
			{
				RaiseGestureDetected(GestureType.SwipeRightToLeft);
				return;
			}

			//LeftToRight
			if (ScanPositions((p1, p2) => Math.Abs(p2.Y - p1.Y) < SwipeMaximalHeight,	// Height
					(p1, p2) => p2.X - p1.X > -0.01f,									// Progression to right
					(p1, p2) => Math.Abs(p2.X - p1.X) > SwipeMinimalLength,				// Length
					SwipeMininalDuration, SwipeMaximalDuration))						// Duration
			{
				RaiseGestureDetected(GestureType.SwipeLeftToRight);
				return;
			}

			//Top to Bottom
			if (ScanPositions((p1, p2) => Math.Abs(p2.X - p1.X) < SwipeMaximalHeight,	// Height
					(p1, p2) => p2.Y - p1.Y < 0.01f,									// Progression to right
					(p1, p2) => (p2.Y - p1.Y) > SwipeMinimalLength,						// Length
					SwipeMininalDuration, SwipeMaximalDuration))						// Duration
			{
				RaiseGestureDetected(GestureType.SwipeTopToBottom);
				return;
			}

			//Bottom to Top 
			if (ScanPositions((p1, p2) => Math.Abs(p2.X - p1.X) < SwipeMaximalHeight,	// Height
					(p1, p2) => p2.Y - p1.Y > -0.01f,									// Progression to right
					(p1, p2) => Math.Abs(p2.Y - p1.Y) > SwipeMinimalLength,						// Length
					SwipeMininalDuration, SwipeMaximalDuration))						// Duration
			{
				RaiseGestureDetected(GestureType.SwipeBottomToTop);
				return;
			}
		}
        
    }
}
