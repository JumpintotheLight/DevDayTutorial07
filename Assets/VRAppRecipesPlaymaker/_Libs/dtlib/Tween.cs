using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ZefirVR {

	public class Tween : MonoBehaviour {
		public enum EaseType {
			linear,
			clerp,
			spring,
			easeInQuad,
			easeOutQuad,
			easeInOutQuad,
			easeInCubic,
			easeOutCubic,
			easeInOutCubic,
			easeInQuart,
			easeOutQuart,
			easeInOutQuart,
			easeInQuint,
			easeOutQuint,
			easeInOutQuint,
			easeInSine,
			easeOutSine,
			easeInOutSine,
			easeInExpo,
			easeOutExpo,
			easeInOutExpo,
			easeInCirc,
			easeOutCirc,
			easeInOutCirc,
			easeInBounce,
			easeOutBounce,
			easeInOutBounce,
			easeInBack,
			easeOutBack,
			easeInOutBack,
			easeInElastic,
			easeOutElastic,
			easeInOutElastic
		}

		public static Tween instance;
		static bool initialized = false;
		static Base[] tweens = new Base[100];
		static int count = 0;
		static float deltaTime, realDeltaTime;

		void Update() {
			unchecked {
				if (count > 0) {
					deltaTime = Time.deltaTime;
					realDeltaTime = Time.unscaledDeltaTime;
					if (realDeltaTime > 0.1f)
						realDeltaTime = 0.1f;
					int d = 0, cnt = count;
					for (int i=0; i<count; i++) {
						Base b = tweens[i];
						if (!b.stop) {
							if (i < cnt)
								b.Update();
							if (i != d)
								tweens[d] = b;
							d++;
						}
					}
					if (count != d) {
						for (int i=d; i<count; i++)
							tweens[i] = null;
						count = d;
					}
				}
			}
		}

		public static Base[] GetTweens(out int count) {
			count = Tween.count;
			return tweens;
		}

		public static void StopAll() {
			for (int i=0; i<count; i++)
				tweens[i].Stop();
		}

		public static void Stop<T>(Tween.Base tween, object target) {
			if (target != null)
				for (int i=0; i<count; i++) {
					Base t = tweens[i];
					if (!t.stop && t.GetType() == typeof(T) && t != tween && t.target == target)
						t.Stop();
				}
		}

		public static void Stop(object target) {
			if (target != null)
				for (int i=0; i<count; i++)
					if (tweens[i].target == target && !tweens[i].stop)
						tweens[i].Stop();
		}

		public static void Stop(object target, string tag) {
			if (tag == null) {
				Stop(target);
				return;
			}
			if (target != null)
				for (int i=0; i<count; i++)
					if (tweens[i].tags != null && tweens[i].target == target && !tweens[i].stop && System.Array.IndexOf(tweens[i].tags, tag) >= 0)
						tweens[i].Stop();
		}

		public static void Stop(params string[] tags) {
			foreach (string tag in tags)
				for (int i=0; i<count; i++)
					if (tweens[i].tags != null && !tweens[i].stop && System.Array.IndexOf(tweens[i].tags, tag) >= 0)
						tweens[i].Stop();
		}

		public static void Finish(object target) {
			if (target != null)
				for (int i=0; i<count; i++)
					if (tweens[i].target == target && !tweens[i].stop)
						tweens[i].Finish();
		}

		public static void Finish(object target, string tag) {
			if (tag == null) {
				Finish(target);
				return;
			}
			if (target != null)
				for (int i=0; i<count; i++)
					if (tweens[i].tags != null && tweens[i].target == target && !tweens[i].stop && System.Array.IndexOf(tweens[i].tags, tag) >= 0)
						tweens[i].Finish();
		}

		public static void Iterate(object target, System.Action<Tween.Base> callback) {
			if (callback == null)
				return;
			if (target != null) {
				for (int i=0; i<count; i++)
					if (tweens[i].target == target && !tweens[i].stop)
						callback(tweens[i]);
			} else {
				for (int i=0; i<count; i++)
					if (!tweens[i].stop)
						callback(tweens[i]);
			}
		}

		/*
	 * Pause/Unpause
	 */
		public static void Pause(object target) {
			if (target != null)
				for (int i=0; i<count; i++)
					if (tweens[i].target == target)
						tweens[i].Pause();
		}

		public static void Pause(object target, string tag) {
			if (tag == null) {
				Pause(target);
				return;
			}
			if (target != null)
				for (int i=0; i<count; i++)
					if (tweens[i].tags != null && tweens[i].target == target && !tweens[i].stop && System.Array.IndexOf(tweens[i].tags, tag) >= 0)
						tweens[i].Pause();
		}

		public static void Pause(params string[] tags) {
			foreach (string tag in tags)
				for (int i=0; i<count; i++)
					if (tweens[i].tags != null && !tweens[i].stop && System.Array.IndexOf(tweens[i].tags, tag) >= 0)
						tweens[i].Pause();
		}

		public static void Unpause(object target) {
			if (target != null)
				for (int i=0; i<count; i++)
					if (tweens[i].target == target)
						tweens[i].Unpause();
		}

		public static void Unpause(object target, string tag) {
			if (tag == null) {
				Unpause(target);
				return;
			}
			if (target != null)
				for (int i=0; i<count; i++)
					if (tweens[i].tags != null && tweens[i].target == target && !tweens[i].stop && System.Array.IndexOf(tweens[i].tags, tag) >= 0)
						tweens[i].Unpause();
		}

		public static void Unpause(params string[] tags) {
			foreach (string tag in tags)
				for (int i=0; i<count; i++)
					if (tweens[i].tags != null && !tweens[i].stop && System.Array.IndexOf(tweens[i].tags, tag) >= 0)
						tweens[i].Unpause();
		}

		/*
	 *
	 */
		public static bool IsRunning(object target, string tag) {
			Transform t = null;
			if (target is Component) t = ((Component)target).transform;
			else if (target is GameObject) t = ((GameObject)target).transform;
			if (t != null)
				for (int i=0; i<count; i++)
					if (tweens[i].transform == t && (tag == null || (tweens[i].tags != null && System.Array.IndexOf(tweens[i].tags, tag) >= 0)))
						return true;
			return false;
		}

		public static bool IsRunning(params string[] tags) {
			foreach (string tag in tags)
				for (int i=0; i<count; i++)
					if (tweens[i].tags != null && !tweens[i].stop && System.Array.IndexOf(tweens[i].tags, tag) >= 0)
						return true;
			return false;
		}

		/*
	 * 
	 */
		public static TweenValue Value(float time=1, float delay=0) {
			return new TweenValue() {time=Mathf.Max(time, 0), runningTime=-delay};
		}

		public static TweenMove Move(object target=null, float time=1, float delay=0) {
			return new TweenMove() {target=target, time=Mathf.Max(time, 0), runningTime=-delay};
		}

		public static TweenRotate Rotate(object target=null, float time=1, float delay=0) {
			return new TweenRotate() {target=target, time=Mathf.Max(time, 0), runningTime=-delay};
		}

		public static TweenScale Scale(object target=null, float time=1, float delay=0) {
			return new TweenScale() {target=target, time=Mathf.Max(time, 0), runningTime=-delay};
		}

		public static Base Delay(float time, System.Action action) {
			return new TweenDelay() {time=Mathf.Max(time, 0), onComplete=action};
		}

		/*
	 * 
	 */
		public class Base {
			public bool stop = true, paused = false;
			public string[] tags;
			public float time = .5f, runningTime = 0, percentage = 0, easevalue = 0;
			public bool loop = false, pingpong = false;

			public object target;
			public Transform transform;

			protected System.Func<float, float> ease = Tween.Ease.easeInOutQuad;
			protected bool reverse = false, useRealTime = false;

			public System.Action onStart;
			public System.Action onComplete;
			public System.Action onStop;
			public System.Action onFinal;
			public System.Action onUpdate;
			public System.Action onRepeat;
			public System.Action onReverse;

			protected virtual void Init() {}
			protected virtual void Apply() {}
			protected virtual void Cancel() {}

			public virtual void Reset() {
				stop = true;
				tags = null;
				time = .5f;
				runningTime = percentage = easevalue = 0;
				loop = pingpong = false;
				ease = Tween.Ease.easeInOutQuad;
				reverse = useRealTime = false;

				onStart = onComplete = onStop = onFinal = onUpdate = onRepeat = onReverse = null;
			}

			protected virtual void Updateable() {
				if (count == tweens.Length)
					System.Array.Resize<Base>(ref tweens, tweens.Length * 2);
				tweens[count++] = this;
			}

			public virtual void Update() {
				if (!paused) {
					if (runningTime < 0) {
						if ((runningTime+= (useRealTime ? realDeltaTime : deltaTime)) >= 0) {
							runningTime = 0;
							Started();
						}
						return;
					}
					if (reverse)
						easevalue = 1 - ease(1 - percentage);
					else
						easevalue = ease(percentage);
					Apply();
					if (onUpdate != null)
						onUpdate();

					if ((percentage >= 1 && !reverse) || (percentage <= 0 && reverse)) {
						// completed
						runningTime = 0;
						if (pingpong && !reverse) {
							reverse = true;
							if (onReverse != null)
								onReverse();
						} else if (loop) {
							reverse = false;
							percentage = 0;
							if (onRepeat != null)
								onRepeat();
						} else {
							stop = true;
							if (onComplete != null)
								onComplete();
							if (onFinal != null)
								onFinal();
						}
					} else {
						runningTime+= useRealTime ? realDeltaTime : deltaTime;
						percentage = Mathf.Clamp01(reverse ? 1 - runningTime / time : runningTime / time);
					}
				}
			}

			protected void Started() {
				percentage = 0;
				reverse = false;
				Init();
				if (target != null && this is TweenVector3 && transform != null) {
					for (int i=0; i<count; i++) {
						Base t = tweens[i];
						if (!t.stop && !t.paused && t.GetType() == this.GetType() && (object)t.transform == (object)transform && t != this && ((TweenVector3)t).CheckStop((TweenVector3)this))
							tweens[i].Stop();
					}
				}
				if (time == 0)
					percentage = 1;
				easevalue = ease(percentage);
				Apply();
				if (onStart != null)
					onStart();

				if (time == 0)
					Update();
			}

			public void Pause() {
				paused = true;
			}

			public void Unpause() {
				paused = false;
			}

			public void Stop() {
				Cancel();
				if (!stop) {
					if (onFinal != null)
						onFinal();
					if (onStop != null)
						onStop();
				}
				stop = true;
			}

			public void Finish() {
				if (!stop) {
					percentage = reverse ? 0 : 1;
					Update();
					Stop();
				}
			}

			public Base Start() {
				if (!initialized) {
					GameObject go = GameObject.Find("Tween");
					if (go == null) {
						go = new GameObject("Tween");
						DontDestroyOnLoad(go);
					}
					instance = go.GetComponent<Tween>();
					if (instance == null)
						instance = go.AddComponent<Tween>();
					initialized = true;
				}
				if (target != null && transform == null) {
					if (target is Component) transform = ((Component)target).transform;
					else if (target is GameObject) transform = ((GameObject)target).transform;
				}
				stop = false;
				if (runningTime >= 0)
					Started();
				Updateable();
				return this;
			}

			public void SetProgress(float progress) {
				if (target != null && transform == null) {
					if (target is Component) transform = ((Component)target).transform;
					else if (target is GameObject) transform = ((GameObject)target).transform;
				}
				runningTime = 0;
				reverse = false;
				percentage = Mathf.Clamp01(progress);
				easevalue = ease(percentage);
				Apply();
				if (onUpdate != null)
					onUpdate();
			}

			protected void Callback(Base[] array, int count) {
				for (int i=0; i<count; i++) {
					Base b = array[i];
					if (b.target == null)
						b.target = target;
					b.Start();
				}
			}

			protected void Callback(System.Action[] array, int count) {
				for (int i=0; i<count; i++)
					array[i]();
			}

			protected void Callback<T>(System.Action<T>[] array, int count, T obj) {
				for (int i=0; i<count; i++)
					array[i](obj);
			}

			protected static void AddToArray<T>(ref T[] array, ref int count, T obj) {
				if (array == null) {
					array = new T[1] {obj};
					count = 1;
				} else {
					if (count == array.Length)
						System.Array.Resize<T>(ref array, array.Length * 2);
					array[count++] = obj;
				}
			}

			/*
		 */
			public Base Once {
				get {
					loop = pingpong = false;
					return this;
				}
			}

			public Base Loop {
				get {
					loop = true;
					return this;
				}
			}

			public Base PingPong {
				get {
					pingpong = true;
					return this;
				}
			}

			public Base SetLoop(bool loop) {
				this.loop = loop;
				return this;
			}

			public Base SetPingPong(bool pingpong) {
				this.pingpong = pingpong;
				return this;
			}

			public Base RealTime {
				get {
					this.useRealTime = true;
					return this;
				}
			}

			public Base Ease(System.Func<float, float> fn) {
				this.ease = fn;
				return this;
			}

			public Base Ease(AnimationCurve easeCurve) {
				this.ease = (f) => easeCurve.Evaluate(f);
				return this;
			}

			public Base Ease(EaseType easeType) {
				this.ease = GetEaseFunction(easeType);
				return this;
			}

			public Base Time(float time) {
				this.time = Mathf.Max(time, 0);
				return this;
			}

			public Base Tags(params string[] tags) {
				this.tags = tags;
				return this;
			}

			public Base Target(object t) {
				this.target = t;
				if (target is Component) transform = ((Component)target).transform;
				else if (target is GameObject) transform = ((GameObject)target).transform;
				return this;
			}

			public Base Start(object target) {
				this.target = target;
				return Start();
			}

			public Base Start(object target, float delay) {
				this.target = target;
				this.runningTime = -delay;
				return Start();
			}

			public Base Start(float delay) {
				this.runningTime = -delay;
				return Start();
			}

			public Base OnStart(System.Action callback) {
				onStart = callback;
				return this;
			}

			public Base OnComplete(System.Action callback) {
				onComplete = callback;
				return this;
			}

			public Base OnStop(System.Action callback) {
				onStop = callback;
				return this;
			}

			public Base OnFinal(System.Action callback) {
				onFinal = callback;
				return this;
			}

			public Base OnUpdate(System.Action callback) {
				onUpdate = callback;
				return this;
			}

			public Base OnRepeat(System.Action callback) {
				onRepeat = callback;
				return this;
			}

			public Base OnReverse(System.Action callback) {
				onReverse = callback;
				return this;
			}
		}

		public class TweenDelay : Base {
			public override void Update() {
				unchecked {
					runningTime+= useRealTime ? realDeltaTime : deltaTime;
					if (runningTime >= time) {
						Stop();
						if (onComplete != null)
							onComplete();
						if (onFinal != null)
							onFinal();
					}
				}
			}
		}

		public class TweenValue : Base {
			public float start = 0, end = 1;
			protected System.Action<float> onUpdateFloat;

			public override void Reset() {
				base.Reset();
				start = 0;
				end = 1;
				onUpdateFloat = null;
			}

			public TweenValue() {
				this.ease = Tween.Ease.linear;
			}

			public TweenValue From(float v) {
				start = v;
				return this;
			}

			public TweenValue To(float v) {
				end = v;
				return this;
			}

			protected override void Apply() {
				unchecked {
					easevalue = start + (end - start) * easevalue;
				}
				if (onUpdateFloat != null)
					onUpdateFloat(easevalue);
			}

			public Base OnUpdate(System.Action<float> callback) {
				onUpdateFloat = callback;
				return this;
			}
		}

		public abstract class TweenVector3 : Base {
			protected Vector3 start, end;
			protected Transform startTransform, endTransform;
			protected bool startx = false, starty = false, startz = false, endx = false, endy = false, endz = false;
			protected bool isLocal = true;
			protected bool isAnchored = false;
			protected Vector3 valueVector3;
			protected System.Action<Vector3> onUpdateVector3;

			public override void Reset() {
				base.Reset();
				startTransform = endTransform = null;
				startx = starty = startz = endx = endy = endz = false;
				isLocal = true;
				valueVector3 = Vector3.zero;
				onUpdateVector3 = null;
			}

			public bool CheckStop(TweenVector3 v) {
				return (((startx || endx) && (v.startx || v.endx)) ||
					((starty || endy) && (v.starty || v.endy)) ||
					((startz || endz) && (v.startz || v.endz)));
			}

			public TweenVector3 FromX(float x) {
				start.x = x;
				startx = true;
				return this;
			}

			public TweenVector3 FromY(float y) {
				start.y = y;
				starty = true;
				return this;
			}

			public TweenVector3 FromZ(float z) {
				start.z = z;
				startz = true;
				return this;
			}

			public TweenVector3 From(float x, float y) {
				start.x = x;
				startx = true;
				start.y = y;
				starty = true;
				return this;
			}

			public TweenVector3 From(float x, float y, float z) {
				start.x = x;
				startx = true;
				start.y = y;
				starty = true;
				start.z = z;
				startz = true;
				return this;
			}

			public TweenVector3 From(Vector3 start) {
				this.start = start;
				startx = true;
				starty = true;
				startz = true;
				return this;
			}

			public TweenVector3 From(Component c) {
				this.startTransform = c.transform;
				return this;
			}

			public TweenVector3 ToX(float x) {
				end.x = x;
				endx = true;
				return this;
			}

			public TweenVector3 ToY(float y) {
				end.y = y;
				endy = true;
				return this;
			}

			public TweenVector3 ToZ(float z) {
				end.z = z;
				endz = true;
				return this;
			}

			public TweenVector3 To(float x, float y) {
				end.x = x;
				endx = true;
				end.y = y;
				endy = true;
				return this;
			}

			public TweenVector3 To(float x, float y, float z) {
				end.x = x;
				endx = true;
				end.y = y;
				endy = true;
				end.z = z;
				endz = true;
				return this;
			}

			public TweenVector3 To(Vector3 end) {
				this.end = end;
				endx = true;
				endy = true;
				endz = true;
				return this;
			}

			public TweenVector3 To(Component c) {
				this.endTransform = c.transform;
				return this;
			}

			public TweenVector3 Local {
				get {
					isLocal = true;
					return this;
				}
			}

			public TweenVector3 Global {
				get {
					isLocal = false;
					return this;
				}
			}

			public TweenVector3 SetLocal(bool isLocal) {
				this.isLocal = isLocal;
				return this;
			}

			public TweenVector3 Anchored {
				get {
					isAnchored = true;
					return this;
				}
			}

			public Base OnUpdate(System.Action<Vector3> callback) {
				onUpdateVector3 = callback;
				return this;
			}

			protected void Init(Vector3 pos) {
				if (!startx) start.x = pos.x;
				if (!starty) start.y = pos.y;
				if (!startz) start.z = pos.z;

				if (!endx) end.x = pos.x;
				if (!endy) end.y = pos.y;
				if (!endz) end.z = pos.z;
				valueVector3 = start;
			}

			protected void Apply(Vector3 pos) {
				unchecked {
					valueVector3 = start + (end - start) * easevalue;
				}
				if (!startx && !endx) valueVector3.x = pos.x;
				if (!starty && !endy) valueVector3.y = pos.y;
				if (!startz && !endz) valueVector3.z = pos.z;
				if (onUpdateVector3 != null)
					onUpdateVector3(valueVector3);
			}
		}

		public class TweenMove : TweenVector3 {
			protected bool snap = false;
			protected RectTransform rectTransform;

			public TweenVector3 Snap {
				get {
					this.snap = true;
					return this;
				}
			}

			public TweenVector3 SetSnap(bool snap) {
				this.snap = snap;
				return this;
			}

			protected override void Init() {
				base.Init();
				if (isAnchored) {
					rectTransform = (RectTransform)transform;
					base.Init(rectTransform.anchoredPosition);
					if ((object)startTransform != null) start = ((RectTransform)startTransform).anchoredPosition;
					if ((object)endTransform != null) end = ((RectTransform)endTransform).anchoredPosition;
				} else {
					base.Init(isLocal ? transform.localPosition : transform.position);
					if ((object)startTransform != null) start = isLocal ? startTransform.localPosition : startTransform.position;
					if ((object)endTransform != null) end = isLocal ? endTransform.localPosition : endTransform.position;
				}
			}

			protected override void Apply() {
				try {
					if (isAnchored) {
						base.Apply(rectTransform.anchoredPosition);
						if (snap) {
							valueVector3.x = Mathf.RoundToInt(valueVector3.x);
							valueVector3.y = Mathf.RoundToInt(valueVector3.y);
							valueVector3.z = Mathf.RoundToInt(valueVector3.z);
						}
						rectTransform.anchoredPosition = valueVector3;
					} else {
						base.Apply(isLocal ? transform.localPosition : transform.position);
						if (snap) {
							valueVector3.x = Mathf.RoundToInt(valueVector3.x);
							valueVector3.y = Mathf.RoundToInt(valueVector3.y);
							valueVector3.z = Mathf.RoundToInt(valueVector3.z);
						}
						if (isLocal)
							transform.localPosition = valueVector3;
						else
							transform.position = valueVector3;
					}
				} catch {
					Debug.Log("target is null on TweenMove");
				}
			}
		}

		public class TweenRotate : TweenVector3 {
			Vector3 aroundPoint, aroundAxis;
			float aroundAngle;
			bool rotateRound = false;
			bool disableFixing = false;

			public override void Reset() {
				base.Reset();
				rotateRound = false;
				disableFixing = false;
			}

			public TweenRotate Around(Vector3 point, Vector3 axis, float angle) {
				aroundPoint = point;
				aroundAxis = axis;
				aroundAngle = angle;
				rotateRound = true;
				return this;
			}

			public TweenRotate DisableAngleFixing {
				get {
					disableFixing = true;
					return this;
				}
			}

			protected override void Init() {
				base.Init();
				base.Init(isLocal ? transform.localEulerAngles : transform.eulerAngles);
				if ((object)startTransform != null) start = isLocal ? startTransform.localEulerAngles : startTransform.eulerAngles;
				if ((object)endTransform != null) end = isLocal ? endTransform.localEulerAngles : endTransform.eulerAngles;
				if (!disableFixing) {
					Vector3 st = start;
					Vector3 en = end;
					float sx0 = Mathf.Abs(st.x - en.x), sx1 = Mathf.Abs((st.x - 360) - en.x), sx2 = Mathf.Abs(st.x - (en.x - 360));
					if (sx1 < sx0 && sx1 < sx2) st.x = st.x - 360;
					else if (sx2 < sx0 && sx2 < sx1) en.x = en.x - 360;

					float sy0 = Mathf.Abs(st.y - en.y), sy1 = Mathf.Abs((st.y - 360) - en.y), sy2 = Mathf.Abs(st.y - (en.y - 360));
					if (sy1 < sy0 && sy1 < sy2) st.y = st.y - 360;
					else if (sy2 < sy0 && sy2 < sy1) en.y = en.y - 360;

					float sz0 = Mathf.Abs(st.z - en.z), sz1 = Mathf.Abs((st.z - 360) - en.z), sz2 = Mathf.Abs(st.z - (en.z - 360));
					if (sz1 < sz0 && sz1 < sz2) st.z = st.z - 360;
					else if (sz2 < sz0 && sz2 < sz1) en.z = en.z - 360;
					start = st;
					end = en;
				}
			}

			protected override void Apply() {
				try {
					if (rotateRound) {
						transform.RotateAround(aroundPoint, aroundAxis, aroundAngle * easevalue);
					} else {
						base.Apply(isLocal ? transform.localEulerAngles : transform.eulerAngles);
						if (isLocal) transform.localEulerAngles = valueVector3;
						else transform.eulerAngles = valueVector3;
					}
				} catch {
					Debug.Log("target is null on TweenRotate");
				}
			}
		}

		public class TweenScale : TweenVector3 {
			protected override void Init() {
				base.Init();
				base.Init(transform.localScale);
				if ((object)startTransform != null) start = startTransform.localScale;
				if ((object)endTransform != null) end = endTransform.localScale;
			}

			protected override void Apply() {
				try {
					base.Apply(transform.localScale);
					transform.localScale = valueVector3;
				} catch {
					Debug.Log("target is null on TweenScale");
				}
			}
		}

		#region Pool
		public class Pool {
			class PoolItem {
				public object[] items;
				public int count;
			}
			Dictionary<System.Type, PoolItem> pool = new Dictionary<System.Type, PoolItem>();
			int poolSize = 32;

			public Pool(int poolSize) {
				this.poolSize = poolSize;
			}

			public T Get<T>(System.Func<T> creator) {
				if (pool.Count > 0) {
					PoolItem poolItem;
					pool.TryGetValue(typeof(T), out poolItem);
					if (poolItem != null) {
						if (poolItem.count > 0) {
							T result = (T)poolItem.items[--poolItem.count];
							poolItem.items[poolItem.count] = null;
							return result;
						}
					}
				}
				return creator();
			}

			public bool Put(object t) {
				PoolItem poolItem;
				pool.TryGetValue(t.GetType(), out poolItem);
				if (poolItem == null) {
					poolItem = new PoolItem() {
						items = new object[poolSize],
						count = 0
					};
					pool[t.GetType()] = poolItem;
				}
				if (poolItem.count < poolItem.items.Length) {
					poolItem.items[poolItem.count++] = t;
					return true;
				}
				return false;
			}
		}

		public class Pool<T> where T : class {
			T[] items;
			int count;
			System.Func<T> callback;

			public Pool(int poolSize, System.Func<T> callback) {
				items = new T[poolSize];
				count = 0;
				this.callback = callback;
			}

			public T Get() {
				if (count > 0) {
					T result = items[--count];
					items[count] = null;
					return result;
				}
				return callback();
			}

			public bool Put(T t) {
				if (count < items.Length) {
					items[count++] = t;
					return true;
				}
				return false;
			}
		}
		#endregion

		#region Ease Functions
		public static System.Func<float, float> GetEaseFunction(EaseType easeType) {
			switch (easeType) {
			case EaseType.linear: return Tween.Ease.linear;
			case EaseType.clerp: return Tween.Ease.clerp;
			case EaseType.spring: return Tween.Ease.spring;
			case EaseType.easeInQuad: return Tween.Ease.easeInQuad;
			case EaseType.easeOutQuad: return Tween.Ease.easeOutQuad;
			case EaseType.easeInOutQuad: return Tween.Ease.easeInOutQuad;
			case EaseType.easeInCubic: return Tween.Ease.easeInCubic;
			case EaseType.easeOutCubic: return Tween.Ease.easeOutCubic;
			case EaseType.easeInOutCubic: return Tween.Ease.easeInOutCubic;
			case EaseType.easeInQuart: return Tween.Ease.easeInQuart;
			case EaseType.easeOutQuart: return Tween.Ease.easeOutQuart;
			case EaseType.easeInOutQuart: return Tween.Ease.easeInOutQuart;
			case EaseType.easeInQuint: return Tween.Ease.easeInQuint;
			case EaseType.easeOutQuint: return Tween.Ease.easeOutQuint;
			case EaseType.easeInOutQuint: return Tween.Ease.easeInOutQuint;
			case EaseType.easeInSine: return Tween.Ease.easeInSine;
			case EaseType.easeOutSine: return Tween.Ease.easeOutSine;
			case EaseType.easeInOutSine: return Tween.Ease.easeInOutSine;
			case EaseType.easeInExpo: return Tween.Ease.easeInExpo;
			case EaseType.easeOutExpo: return Tween.Ease.easeOutExpo;
			case EaseType.easeInOutExpo: return Tween.Ease.easeInOutExpo;
			case EaseType.easeInCirc: return Tween.Ease.easeInCirc;
			case EaseType.easeOutCirc: return Tween.Ease.easeOutCirc;
			case EaseType.easeInOutCirc: return Tween.Ease.easeInOutCirc;
			case EaseType.easeInBounce: return Tween.Ease.easeInBounce;
			case EaseType.easeOutBounce: return Tween.Ease.easeOutBounce;
			case EaseType.easeInOutBounce: return Tween.Ease.easeInOutBounce;
			case EaseType.easeInBack: return Tween.Ease.easeInBack;
			case EaseType.easeOutBack: return Tween.Ease.easeOutBack;
			case EaseType.easeInOutBack: return Tween.Ease.easeInOutBack;
			case EaseType.easeInElastic: return Tween.Ease.easeInElastic;
			case EaseType.easeOutElastic: return Tween.Ease.easeOutElastic;
			case EaseType.easeInOutElastic: return Tween.Ease.easeInOutElastic;
			}
			return Tween.Ease.easeInOutQuad;
		}

		public static class Ease {
			public static float linear(float value) {
				return value;
			}

			public static float clerp(float value) {
				unchecked {
					float min = 0.0f;
					float max = 360.0f;
					float half = Mathf.Abs((max - min) * 0.5f);
					float retval = 0.0f;
					float diff = 0.0f;
					if (1 < -half) {
						diff = (max + 1) * value;
						retval = diff;
					} else if (1 > half) {
						diff = -(max - 1) * value;
						retval = diff;
					} else retval = value;
					return retval;
				}
			}

			public static float spring(float value) {
				unchecked {
					value = Mathf.Clamp01(value);
					value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
					return value;
				}
			}

			public static float easeInQuad(float value) {
				return value * value;
			}

			public static float easeOutQuad(float value) {
				return -value * (value - 2);
			}

			public static float easeInOutQuad(float value) {
				unchecked {
					value/= .5f;
					if (value < 1) return 0.5f * value * value;
					value--;
					return -0.5f * (value * (value - 2) - 1);
				}
			}

			public static float easeInCubic(float value) {
				return value * value * value;
			}

			public static float easeOutCubic(float value) {
				value--;
				return (value * value * value + 1);
			}

			public static float easeInOutCubic(float value) {
				unchecked {
					value /= .5f;
					if (value < 1) return 0.5f * value * value * value;
					value -= 2;
					return 0.5f * (value * value * value + 2);
				}
			}

			public static float easeInQuart(float value) {
				return value * value * value * value;
			}

			public static float easeOutQuart(float value) {
				value--;
				return -(value * value * value * value - 1);
			}

			public static float easeInOutQuart(float value) {
				unchecked {
					value /= .5f;
					if (value < 1) return 0.5f * value * value * value * value;
					value -= 2;
					return -0.5f * (value * value * value * value - 2);
				}
			}

			public static float easeInQuint(float value){
				return value * value * value * value * value;
			}

			public static float easeOutQuint(float value){
				value--;
				return (value * value * value * value * value + 1);
			}

			public static float easeInOutQuint(float value){
				unchecked {
					value /= .5f;
					if (value < 1) return 0.5f * value * value * value * value * value;
					value -= 2;
					return 0.5f * (value * value * value * value * value + 2);
				}
			}

			public static float easeInSine(float value){
				return -Mathf.Cos(value * (Mathf.PI * 0.5f)) + 1;
			}

			public static float easeOutSine(float value){
				return Mathf.Sin(value * (Mathf.PI * 0.5f));
			}

			public static float easeInOutSine(float value){
				return -0.5f * (Mathf.Cos(Mathf.PI * value) - 1);
			}

			public static float easeInExpo(float value){
				return Mathf.Pow(2, 10 * (value - 1));
			}

			public static float easeOutExpo(float value){
				return (-Mathf.Pow(2, -10 * value ) + 1);
			}

			public static float easeInOutExpo(float value){
				value /= .5f;
				if (value < 1) return 0.5f * Mathf.Pow(2, 10 * (value - 1));
				value--;
				return 0.5f * (-Mathf.Pow(2, -10 * value) + 2);
			}

			public static float easeInCirc(float value){
				return -(Mathf.Sqrt(1 - value * value) - 1);
			}

			public static float easeOutCirc(float value){
				value--;
				return Mathf.Sqrt(1 - value * value);
			}

			public static float easeInOutCirc(float value){
				unchecked {
					value /= .5f;
					if (value < 1) return -0.5f * (Mathf.Sqrt(1 - value * value) - 1);
					value -= 2;
					return 0.5f * (Mathf.Sqrt(1 - value * value) + 1);
				}
			}

			/* GFX47 MOD START */
			public static float easeInBounce(float value){
				float d = 1f;
				return easeOutBounce(d-value);
			}
			/* GFX47 MOD END */

			/* GFX47 MOD START */
			//private float bounce(float value){
			public static float easeOutBounce(float value){
				value /= 1f;
				if (value < (1 / 2.75f)){
					return (7.5625f * value * value);
				}else if (value < (2 / 2.75f)){
					value -= (1.5f / 2.75f);
					return (7.5625f * (value) * value + .75f);
				}else if (value < (2.5 / 2.75)){
					value -= (2.25f / 2.75f);
					return (7.5625f * (value) * value + .9375f);
				}else{
					value -= (2.625f / 2.75f);
					return (7.5625f * (value) * value + .984375f);
				}
			}
			/* GFX47 MOD END */

			/* GFX47 MOD START */
			public static float easeInOutBounce(float value){
				float d = 1f;
				if (value < d* 0.5f) return easeInBounce(value*2) * 0.5f;
				else return easeOutBounce(value*2-d) * 0.5f + 0.5f;
			}
			/* GFX47 MOD END */

			public static float easeInBack(float value){
				value /= 1;
				float s = 1.70158f;
				return value * value * ((s + 1) * value - s);
			}

			public static float easeOutBack(float value){
				float s = 1.70158f;
				value = (value) - 1;
				return ((value) * value * ((s + 1) * value + s) + 1);
			}

			public static float easeInOutBack(float value){
				float s = 1.70158f;
				value /= .5f;
				if ((value) < 1){
					s *= (1.525f);
					return 0.5f * (value * value * (((s) + 1) * value - s));
				}
				value -= 2;
				s *= (1.525f);
				return 0.5f * ((value) * value * (((s) + 1) * value + s) + 2);
			}

			/*static float punch(float amplitude, float value){
			float s = 9;
			if (value == 0){
				return 0;
			}
			else if (value == 1){
				return 0;
			}
			float period = 1 * 0.3f;
			s = period / (2 * Mathf.PI) * Mathf.Asin(0);
			return (amplitude * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * 1 - s) * (2 * Mathf.PI) / period));
		}*/

			/* GFX47 MOD START */
			public static float easeInElastic(float value){
				float d = 1f;
				float p = d * .3f;
				float s = 0;
				float a = 0;

				if (value == 0) return 0;

				if ((value /= d) == 1) return 1;

				if (a == 0f || a < 1){
					a = 1;
					s = p / 4;
				}else{
					s = p / (2 * Mathf.PI) * Mathf.Asin(1 / a);
				}

				return -(a * Mathf.Pow(2, 10 * (value-=1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p));
			}		
			/* GFX47 MOD END */

			/* GFX47 MOD START */
			//private float elastic(float value){
			public static float easeOutElastic(float value){
				/* GFX47 MOD END */
				//Thank you to rafael.marteleto for fixing this as a port over from Pedro's UnityTween
				float d = 1f;
				float p = d * .3f;
				float s = 0;
				float a = 0;

				if (value == 0) return 0;

				if ((value /= d) == 1) return 1;

				if (a == 0f || a < 1){
					a = 1;
					s = p * 0.25f;
				}else{
					s = p / (2 * Mathf.PI) * Mathf.Asin(1 / a);
				}

				return (a * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) + 1);
			}		

			/* GFX47 MOD START */
			public static float easeInOutElastic(float value){
				float d = 1f;
				float p = d * .3f;
				float s = 0;
				float a = 0;

				if (value == 0) return 0;

				if ((value /= d*0.5f) == 2) return 1;

				if (a == 0f || a < 1){
					a = 1;
					s = p / 4;
				}else{
					s = p / (2 * Mathf.PI) * Mathf.Asin(1 / a);
				}

				if (value < 1) return -0.5f * (a * Mathf.Pow(2, 10 * (value-=1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p));
				return a * Mathf.Pow(2, -10 * (value-=1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) * 0.5f + 1;
			}		
			/* GFX47 MOD END */
		}
		#endregion
	}


}
