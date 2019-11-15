namespace d4160.Core.Editors
{
	/*
		Reorder Components
		by Arthur "Mr. Podunkian" Lee (@MrPodunkian)
	
		This program is free software: you can redistribute it and/or modify
		it under the terms of the GNU General Public License as published by
		the Free Software Foundation, either version 3 of the License, or
		any later version.

		This program is distributed in the hope that it will be useful,
		but WITHOUT ANY WARRANTY; without even the implied warranty of
		MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
		GNU General Public License for more details.

		You should have received a copy of the GNU General Public License
		along with this program.  If not, see <http://www.gnu.org/licenses/>.
	*/

	using UnityEngine;
	using UnityEditor;
	using System.Reflection;
	using System.Collections;
	using System.Collections.Generic;

	[ExecuteInEditMode]
	public class ReorderComponentsWindow : EditorWindow
	{
		public static Texture2D separatorTexture;
		public static GUIStyle separator;

		public static Texture2D destinationTexture;
		public static GUIStyle destination;

		protected ComponentDisplay _selectedDisplay;

		protected Dictionary<Component, ComponentDisplay> _componentDisplays;
		protected Vector2 _scrollPosition;
		protected List<Component> _cachedComponents;

		protected bool _dragged = false;
		protected bool _initialized;

		public class ComponentDisplay
		{
			public Component component;
			protected Rect _rect;
			protected Rect _contentRect;
			protected bool _opened = true;

			public void Toggle()
			{
				_opened = !_opened;
			}

			public void Draw(bool is_selected)
			{
				_rect = EditorGUILayout.BeginVertical();

				GUILayout.Box(separatorTexture, separator);

				if (!is_selected)
				{
					GUI.color = new Color(1.0F, 1.0F, 1.0F, 1.0F);
				}
				else
				{
					GUI.color = new Color(1.0F, 1.0F, 1.0F, 0.25F);
				}

				EditorGUILayout.BeginVertical();

				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(4.0F);

				// Create an area for the foldout, and then create the foldout itself.
				GUILayout.Box("", new GUIStyle(), GUILayout.Width(12.0F));
				_opened = EditorGUI.Foldout(GUILayoutUtility.GetLastRect(), _opened, "");

				GUIContent content = EditorGUIUtility.ObjectContent(component, component.GetType());

				GUILayout.Box("", new GUIStyle(), GUILayout.Width(16.0F), GUILayout.Height(16.0F));

				GUI.DrawTexture(GUILayoutUtility.GetLastRect(), content.image);// GUILayout.Label(content.image, GUILayout.MaxHeight(18.0F), GUILayout.MaxWidth(20.0F));

				PropertyInfo enabled_property = component.GetType().GetProperty("enabled");

				if (enabled_property != null && enabled_property.PropertyType == typeof(bool)) // Draw the enable/disable toggle if available
				{
					bool enabled = (bool)enabled_property.GetValue(component, null);
					GUILayout.Box("", new GUIStyle(), GUILayout.Width(12.0F));
					enabled_property.SetValue(component, GUI.Toggle(GUILayoutUtility.GetLastRect(), enabled, ""), null);
				}
				else
				{
					GUILayout.Space(12.0F);
				}

				EditorGUILayout.LabelField(component.GetType().Name, EditorStyles.boldLabel);

				EditorGUILayout.EndHorizontal();

				_contentRect = EditorGUILayout.BeginVertical();

				if (_opened)
				{
					EditorGUILayout.BeginHorizontal();
					GUILayout.Space(12.0F);
					EditorGUILayout.BeginVertical();

					Editor temp = Editor.CreateEditor(component);
					temp.OnInspectorGUI();

					EditorGUILayout.EndHorizontal();
					EditorGUILayout.EndVertical();
				}

				EditorGUILayout.EndVertical();

				EditorGUILayout.Space();
				EditorGUILayout.EndVertical();
				EditorGUILayout.EndVertical();

				if (is_selected)
				{
					EditorGUILayout.EndFadeGroup();
				}

				GUI.color = new Color(1.0F, 1.0F, 1.0F, 1.0F);
			}

			public Rect GetContentRect()
			{
				return _contentRect;
			}

			public Rect GetRect()
			{
				return _rect;
			}
		}

		[MenuItem("Window/Game Framework/Reorder Components")]
		private static void ShowWindow ()
		{
			ReorderComponentsWindow window = EditorWindow.GetWindow<ReorderComponentsWindow>(false, "Reorder Components", true);
		}

		void OnInspectorUpdate()
		{
			Repaint();
		}

		public void MoveToTop(object obj)
		{
			Component component = (Component)obj;

			for (int i = 0; i < _cachedComponents.IndexOf(component); i++)
			{
				UnityEditorInternal.ComponentUtility.MoveComponentUp(component);
			}
		}

		public void MoveToBottom(object obj)
		{
			Component component = (Component)obj;

			for (int i = _cachedComponents.IndexOf(component); i < _cachedComponents.Count; i++)
			{
				UnityEditorInternal.ComponentUtility.MoveComponentDown(component);
			}
		}

		public void MoveUp(object obj)
		{
			UnityEditorInternal.ComponentUtility.MoveComponentUp((Component)obj);
		}

		public void MoveDown(object obj)
		{
			UnityEditorInternal.ComponentUtility.MoveComponentDown((Component)obj);
		}

		public void Initialize()
		{
			separatorTexture = new Texture2D(1, 1);
			separatorTexture.SetPixel(0, 0, new Color(0.3F, 0.3F, 0.3F, 1.0F));
			separatorTexture.Apply();

			separator = new GUIStyle();
			separator.fixedHeight = 1.0F;
			separator.normal.background = separatorTexture;

			destinationTexture = new Texture2D(1, 1);
			destinationTexture .SetPixel(0, 0, new Color(0.0F, 5.0F, 1.0F, 1.0F));
			destinationTexture .Apply();

			destination = new GUIStyle();
			destination.fixedHeight = 1.0F;
			destination.normal.background = destinationTexture;
			_initialized = true;
		}

		public void OnGUI ()
		{
			if (EditorApplication.isCompiling || EditorApplication.isUpdating)
			{
				EditorGUILayout.LabelField("Please wait...", EditorStyles.centeredGreyMiniLabel);

				_cachedComponents = null;
				_selectedDisplay = null;
				_componentDisplays = null;
				_initialized = false;
				return;
			}

			if (!_initialized)
			{
				Initialize();
			}

			Transform transform = Selection.activeTransform;

			if (Selection.GetFiltered(typeof(Transform), SelectionMode.Unfiltered).Length > 0)
			{
				transform = (Transform)Selection.GetFiltered(typeof(Transform), SelectionMode.Unfiltered)[0];
			}

			if (transform == null)
			{
				EditorGUILayout.LabelField("Select a GameObject.", EditorStyles.centeredGreyMiniLabel);
				_cachedComponents = null;
				_selectedDisplay = null;
				_componentDisplays = null;
			}
			else
			{
				bool dirty = false;

				Component[] components = transform.GetComponents<Component>();

				if (_cachedComponents == null || _cachedComponents.Count != components.Length)
				{
					dirty = true;
				}
				else
				{
					for (int i = 0; i < components.Length; i++)
					{
						if (components[i] != _cachedComponents[i])
						{
							dirty = true;
							break;
						}
					}
				}

				if (dirty)
				{
					Dictionary<Component, ComponentDisplay> displays = new Dictionary<Component, ComponentDisplay>();
					_cachedComponents = new List<Component>(components);

					if (_componentDisplays == null)
					{
						_componentDisplays = new Dictionary<Component, ComponentDisplay>();
					}

					for (int i = 0; i < _cachedComponents.Count; i++)
					{
						Component component = _cachedComponents[i];

						if (component == null)
						{
							continue;
						}

						ComponentDisplay display = null;

						if (!_componentDisplays.TryGetValue(component, out display))
						{
							display = new ComponentDisplay();
							display.component = component;
						}

						displays[component] = display;
					}

					_componentDisplays = displays;
				}
			}

			if (_componentDisplays == null)
			{
				return;
			}

			_scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

			Editor editor = Editor.CreateEditor(transform.gameObject);
			editor.DrawHeader();

			if (transform != null)
			{
				for (int i = 0; i < _cachedComponents.Count; i++)
				{
					Component component = _cachedComponents[i];

					_componentDisplays[component].Draw(_componentDisplays[component] == _selectedDisplay);
				}

				ComponentDisplay hovered_display = null;

				for (int i = 0; i < _cachedComponents.Count; i++)
				{
					Component component = _cachedComponents[i];

					if (_componentDisplays[component].GetRect().Contains(Event.current.mousePosition))
					{
						hovered_display = _componentDisplays[component];
						break;
					}
				}

				if (hovered_display != null)
				{
					if (Event.current.type == EventType.ContextClick)
					{
						GenericMenu menu = new GenericMenu();
						menu.AddItem(new GUIContent("Move Up"), false, MoveUp, hovered_display.component);
						menu.AddItem(new GUIContent("Move Down"), false, MoveDown, hovered_display.component);

						menu.AddItem(new GUIContent("Move to Top"), false, MoveToTop, hovered_display.component);
						menu.AddItem(new GUIContent("Move to Bottom"), false, MoveToBottom, hovered_display.component);
						menu.ShowAsContext();
						Event.current.Use();
					}

					if (_selectedDisplay == null && Event.current.type == EventType.MouseDown && Event.current.button == 0)
					{
						_dragged = false;
						_selectedDisplay = hovered_display;
						Event.current.Use();
					}
				}

				if (_selectedDisplay != null) // Something was selected.
				{
					// If we're hovering over an option that's not the one we started on, treat this as a drag.
					if (hovered_display != null && _selectedDisplay != hovered_display)
					{
						_dragged = true;
					}

					// Draw where the component will be dropped.

					if (hovered_display != null)
					{
						Rect rect = new Rect(hovered_display.GetRect());
						rect.height = 1;

						GUI.Box(rect, "", destination);
					}
					else if (Event.current.mousePosition.y > _componentDisplays[_cachedComponents[_cachedComponents.Count - 1]].GetRect().y)
					{
						Rect rect = new Rect(_componentDisplays[_cachedComponents[_cachedComponents.Count - 1]].GetRect());
						rect.y += rect.height;
						rect.height = 1;

						GUI.Box(rect, "", destination);
					}
					else
					{
						Rect rect = new Rect(_componentDisplays[_cachedComponents[0]].GetRect());
						rect.height = 1;

						GUI.Box(rect, "", destination);
					}
				}

				if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
				{
					if (_selectedDisplay != null && _selectedDisplay == hovered_display)
					{
						// If we didn't attempt to drag the component, and we're not over content, fold it out.
						if (!_dragged && !_selectedDisplay.GetContentRect().Contains(Event.current.mousePosition))
						{
							_selectedDisplay.Toggle();
						}

						_selectedDisplay = null;
						Event.current.Use();
					}
					else if (_selectedDisplay != null)
					{
						if (hovered_display != null)
						{
							int source_index = _cachedComponents.IndexOf(_selectedDisplay.component);
							int destination_index = _cachedComponents.IndexOf(hovered_display.component);

							int offset = destination_index - source_index;

							if (destination_index > source_index)
							{
								offset -= 1;
							}

							while (offset > 0)
							{
								UnityEditorInternal.ComponentUtility.MoveComponentDown(_selectedDisplay.component);
								offset --;
							}
							while (offset < 0)
							{
								UnityEditorInternal.ComponentUtility.MoveComponentUp(_selectedDisplay.component);
								offset ++;
							}
						}
						else if (Event.current.mousePosition.y > _componentDisplays[_cachedComponents[_cachedComponents.Count - 1]].GetRect().y)
						{
							MoveToBottom(_selectedDisplay.component);
						}
						else
						{
							MoveToTop(_selectedDisplay.component);
						}

						_selectedDisplay = null;
					}

					Event.current.Use();
				}
			}

			EditorGUILayout.EndScrollView();
		}
	}
}