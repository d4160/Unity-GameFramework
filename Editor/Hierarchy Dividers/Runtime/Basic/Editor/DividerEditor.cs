#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using ManasparkAssets.HierarchyDividers;

namespace ManasparkAssets
{
	namespace HierarchyDividers
	{
		public class DividerEditor : EditorWindow
		{
			private DividerPreset_SCO dividerPreset;
			private int setID;
			private SerializedObject serializedDividerPreset;
			private ReorderableList list;

			private static DividerEditor window;
			private float minHeight = 1.5f * EditorGUIUtility.singleLineHeight;
			private static int windowWidth = 280;

			private int contentWidth
			{
				get { return windowWidth - 80; }
			}

			private Vector2 scrollPos;
			
			[MenuItem("Window/Manaspark/Hierarchy Divider Config (free)")]
			static void Init()
			{
				window = (DividerEditor) EditorWindow.GetWindow(typeof(DividerEditor));
				window.titleContent = new GUIContent("Hrchy Dividers");
				window.minSize = new Vector2(windowWidth, 175f);
				window.Show();
			}
			
			[UnityEditor.Callbacks.OnOpenAsset(1)]
			public static bool OnOpenAsset(int instanceID, int line)
			{
				if (Selection.activeObject as DividerPreset_SCO != null) {
					Init();
					return true; //catch open file
				}        
     
				return false; // let unity open the file
			}

			
			private void OnEnable()
			{
				createList();
				HierarchyDividerDrawing.LoadMaterials();
			}

			private void createList()
			{
				if (serializedDividerPreset != null)
				{
					SerializedProperty listProp = serializedDividerPreset.FindProperty("Dividers");
					list = new ReorderableList(serializedDividerPreset, listProp);
					heights = new List<float>(listProp.arraySize);
					subscribeListDrawing(list);
					subscribeElementHeightCallback(list);
					subscribeOnMouseUpCallback(list);
					subscribeHeaderDrawing(list);
				}
			}
			
			void OnGUI()
			{
				EditorGUILayout.Space();
				EditorGUILayout.BeginHorizontal();

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Preset:", GUILayout.Width(50f));
				dividerPreset = (DividerPreset_SCO) EditorGUILayout.ObjectField(dividerPreset, typeof(DividerPreset_SCO), false);
				
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();
				
				if (dividerPreset == null)
				{
					EditorGUILayout.HelpBox("Assign a preset.", MessageType.Info);
					serializedDividerPreset = null;
					list = null;
					return;
				}

				if (setID != dividerPreset.GetInstanceID())
				{
					serializedDividerPreset = null;
					list = null;
				}
				setID = dividerPreset.GetInstanceID();
								
				if(serializedDividerPreset == null)
					serializedDividerPreset = new SerializedObject(dividerPreset);
				if(list == null)
					createList();
				
				if (serializedDividerPreset != null)
				{
					if(list == null)
						createList();
					
					serializedDividerPreset.Update();
					
					EditorGUILayout.BeginHorizontal();
					if (GUILayout.Button("Expand all", EditorStyles.miniButton))
					{
						int c = dividerPreset.Dividers.Count;
						for (int i = 0; i < c; i++)
						{
							dividerPreset.Dividers[i].foldOut = true;
						}
					}
					if (GUILayout.Button("Collapse all", EditorStyles.miniButton))
					{
						int c = dividerPreset.Dividers.Count;
						for (int i = 0; i < c; i++)
						{
							dividerPreset.Dividers[i].foldOut = false;
						}
					}
					EditorGUILayout.EndHorizontal();
					
					scrollPos =
						EditorGUILayout.BeginScrollView(scrollPos, false, false);
					
					list.DoLayoutList();
					serializedDividerPreset.ApplyModifiedProperties();

					EditorGUILayout.EndScrollView();
					
					if(GUILayout.Button("Place dividers"))
						PlaceDividers();
					if(GUILayout.Button("Delete dividers"))
						DeleteAllDividers();
										
					EditorUtility.SetDirty(dividerPreset);
				}				
			}

			void subscribeListDrawing(ReorderableList list)
			{
				list.drawElementCallback = 
					(Rect rect, int index, bool isActive, bool isFocused) => {
						if(dividerPreset != null && dividerPreset.Dividers[index] != null)
							setBaseValues(dividerPreset.Dividers[index]);
						SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
						
						float height = minHeight;

						float rightOffset = 33;
						rect.x += (EditorGUILayout.GetControlRect().width - contentWidth - rightOffset - 10f) / 2f ;
						
						addHeight(ref height, ref rect, 5);
						
						bool foldoutButton = EditorGUI.DropdownButton(new Rect(rect.x - 2, rect.y - 2, contentWidth + rightOffset, EditorGUIUtility.singleLineHeight), new GUIContent(""), FocusType.Keyboard, EditorStyles.toolbar);
						bool foldout = dividerPreset.Dividers[index].foldOut;
						foldout = foldoutButton ? !foldout : foldout;
						dividerPreset.Dividers[index].foldOut = foldout;
						
						string _label = element.FindPropertyRelative("Label").stringValue;
						_label = element.FindPropertyRelative("Name").stringValue == "" ? "*new*" : _label;
						
						GUIStyle style = new GUIStyle(EditorStyles.label);
						style.alignment = TextAnchor.MiddleCenter;
						
						if (dividerPreset.Dividers[index].ColoredFont)
							style.normal.textColor = dividerPreset.Dividers[index].FontColor;

						if (!dividerPreset.Dividers[index].BoldFont && !dividerPreset.Dividers[index].ItalicFont)
							style.fontStyle = FontStyle.Normal;
						if (dividerPreset.Dividers[index].BoldFont && !dividerPreset.Dividers[index].ItalicFont)
							style.fontStyle = FontStyle.Bold;
						else if(!dividerPreset.Dividers[index].BoldFont && dividerPreset.Dividers[index].ItalicFont)
							style.fontStyle = FontStyle.Italic;
						else if(dividerPreset.Dividers[index].BoldFont && dividerPreset.Dividers[index].ItalicFont)
							style.fontStyle = FontStyle.BoldAndItalic;

						style.alignment = TextAnchor.MiddleCenter;

						float _headerSpriteSize = 13f;

						if (dividerPreset.Dividers[index].ColoredBG)
							drawRect(new Rect(rect.x + rightOffset, rect.y, contentWidth - rightOffset, rect.height), dividerPreset.Dividers[index].BGColor, EditorGUIUtility.singleLineHeight - 2);

						if (dividerPreset.Dividers[index].HasSprite_left &&
						    dividerPreset.Dividers[index].LeftSprite != null)
						{
							HierarchyDividerDrawing
								.DrawSprite(new Rect(rect.x + 14, rect.y - 1f, _headerSpriteSize, _headerSpriteSize),
								            dividerPreset.Dividers[index].LeftSprite.texture, _headerSpriteSize);
						}

						if (dividerPreset.Dividers[index].HasSprite_right &&
						    dividerPreset.Dividers[index].RightSprite != null)
						{
							HierarchyDividerDrawing
								.DrawSprite(new Rect(rect.x + contentWidth + 7, rect.y - 1f, _headerSpriteSize, _headerSpriteSize),
								            dividerPreset.Dividers[index].RightSprite.texture, _headerSpriteSize);
						}

						EditorGUI.LabelField(new Rect(rect.x + rightOffset, rect.y, contentWidth - rightOffset, EditorGUIUtility.singleLineHeight), _label, style);
						
						EditorGUI.Foldout(new Rect(rect.x + 1f, rect.y, contentWidth + rightOffset, EditorGUIUtility.singleLineHeight), foldout, "", EditorStyles.foldout);

						if (foldout)
						{				
							addHeight(ref height, ref rect, EditorGUIUtility.singleLineHeight);
							EditorGUI.LabelField(new Rect(rect.x - 2, rect.y - 1, contentWidth + rightOffset, 175), "", EditorStyles.textArea);
							
							addHeight(ref height, ref rect, 4);
						
							SinglePropertyField(ref rect, ref height, element, "Name", "Text", 50);
							
							InlinePropertyField(rect, 0f, contentWidth - 90f, element, "PreSuffix", "Pre-/Suffix", 70);
							InlinePropertyField(rect, contentWidth - 85f, 85f, element, "PreSuffixCount", "amount", 50);
							addHeight(ref height, ref rect, minHeight);

							InlinePropertyField(rect, 0f, contentWidth - 120f, element, "BoldFont", "bold", 50);
							InlinePropertyField(rect, contentWidth - 115f, 115, element, "ItalicFont", "italic", 50);
							addHeight(ref height, ref rect, minHeight);

							InlinePropertyField(rect, 0f, contentWidth - 120f, element, "ColoredFont", "colored", 50);
							InlinePropertyField(rect, contentWidth - 115f, 115f, element, "FontColor", "font color",
								60);
							addHeight(ref height, ref rect, minHeight);

							InlinePropertyField(rect, 0f, contentWidth - 110f, element, "ColoredBG", "colored bg", 65);
							InlinePropertyField(rect, contentWidth - 105f, 105f, element, "BGColor", "bg color", 50);
							addHeight(ref height, ref rect, minHeight);

							float _spriteSize = EditorGUIUtility.singleLineHeight * 1.25f;

							InlinePropertyField(rect, 0f, 15f, element, "HasSprite_left");
							InlinePropertyField(rect, 20f, contentWidth - 20f, element, "LeftSprite", "left Sprite", 70);
							
							Sprite leftSprite = dividerPreset.Dividers[index].LeftSprite;
							if (leftSprite != null)
							{
								HierarchyDividerDrawing
									.DrawSprite(new Rect(rect.x + contentWidth + 5f, rect.y, _spriteSize, _spriteSize),
									            leftSprite.texture, _spriteSize);
							}
															
							addHeight(ref height, ref rect, minHeight);

							InlinePropertyField(rect, 0f, 15f, element, "HasSprite_right");
							InlinePropertyField(rect, 20f, contentWidth - 20f, element, "RightSprite", "right Sprite",
								70);

							Sprite rightSprite = dividerPreset.Dividers[index].RightSprite;
							if (rightSprite != null)
							{
								HierarchyDividerDrawing
									.DrawSprite(new Rect(rect.x + contentWidth + 5f, rect.y, _spriteSize, _spriteSize),
									            rightSprite.texture, _spriteSize);
							}
															
							addHeight(ref height, ref rect, 5f);
						}
						
						dividerPreset.Dividers[index].SetLabel();

						SetListElementHeight(index, height, rect);
					};
			}

			void subscribeHeaderDrawing(ReorderableList list)
			{
				list.drawHeaderCallback = (Rect rect) => {
					EditorGUI.LabelField(rect, "Dividers");
				};
			}
			
			List<float> heights;
			
			private int lastSelectedIndex = -1;
			void subscribeOnMouseUpCallback(ReorderableList list)
			{
				list.onMouseUpCallback = (ReorderableList _list) =>
				{
					if (_list.index == lastSelectedIndex)
					{
						list.index = -1;
						lastSelectedIndex = -1;
					}
					else
					{
						lastSelectedIndex = list.index;
					}
				};
			}
			
			void subscribeElementHeightCallback(ReorderableList list)
			{
				list.elementHeightCallback = (index) =>
				{
					Repaint();
					float height = 0f;

					try
					{
						height = heights[index];
					}
					catch (ArgumentOutOfRangeException)
					{
						//Debug.LogWarning(e.Message);
					}
					finally
					{
						float[] floats = heights.ToArray();
						Array.Resize(ref floats, serializedDividerPreset.FindProperty("Dividers").arraySize);
						heights = floats.ToList();
					}

					return height;
				};
			}

			void addHeight(ref float h, ref Rect r, float val)
			{
				r.y += val;
				h += val;
			}
			
			void SetListElementHeight(int index, float height, Rect rect)
			{
				try
				{
					heights[index] = height;
				}
				catch (ArgumentOutOfRangeException e)
				{
					Debug.Log(e.Message);
				}
				finally
				{
					float[] floats = heights.ToArray();
					Array.Resize(ref floats, serializedDividerPreset.FindProperty("Dividers").arraySize);
					heights = floats.ToList();
				}
			}

			void SinglePropertyField(ref Rect _rect, ref float height, SerializedProperty e, string _propName, string _label = null, int _labelwidth = 0)
			{
				float lw = EditorGUIUtility.labelWidth;
				EditorGUIUtility.labelWidth = _labelwidth != 0 ? _labelwidth : lw;
				
				GUIContent gc = _label == null ? GUIContent.none : new GUIContent(_label);
				EditorGUI.PropertyField(
					new Rect(_rect.x, _rect.y, contentWidth, EditorGUIUtility.singleLineHeight),
					e.FindPropertyRelative(_propName), gc);
				addHeight(ref height, ref _rect, minHeight);
				
				EditorGUIUtility.labelWidth = lw;
			}
			
			void InlinePropertyField(Rect _rect, float _x, float width, SerializedProperty e, string _propName, string _label = null, int _labelwidth = 0)
			{
				float lw = EditorGUIUtility.labelWidth;
				EditorGUIUtility.labelWidth = _labelwidth != 0 ? _labelwidth : lw;
				
				GUIContent gc = _label == null ? GUIContent.none : new GUIContent(_label);
				EditorGUI.PropertyField(
					new Rect(_rect.x + _x, _rect.y, width, EditorGUIUtility.singleLineHeight),
					e.FindPropertyRelative(_propName), gc);
				
				EditorGUIUtility.labelWidth = lw;
			}
			
			void drawRect(Rect rect, Color Col, float height)
			{
				Rect _r = new Rect(rect);
				_r.height = height;
				Texture2D tex = new Texture2D(1, 1);
				Color bgCol = new Color();
				bgCol.r = Col.r; bgCol.g = Col.g; bgCol.b = Col.b; bgCol.a = Col.a;
				tex.SetPixel(0, 0, bgCol);
				tex.Apply();
				GUI.DrawTexture(_r, tex as Texture);
			}

			void setBaseValues(DividerPreset_SCO.Divider _divider)
			{
				_divider.HasOverrideColor_leftSprite = _divider.HasOverrideColor_rightSprite = false;
				_divider.LeftScale = _divider.RightScale = 1f;
			}

			struct hcInfo
			{
				public string Label;
				public int Index;

				public hcInfo(string l, int i)
				{
					Label = l;
					Index = i;
				}
			}
			
			void PlaceDividers()
			{
				if(HierarchyDividerDrawing.Dividers == null)
					HierarchyDividerDrawing.Dividers = new List<HierarchyDividerValues>();
				
				MonoBehaviour[] _sceneComponents = FindObjectsOfType<MonoBehaviour>();
				IHierarchyDivider[] hds = _sceneComponents
				                          .Where(x => x.GetComponent<IHierarchyDivider>() != null)
				                          .Select(x => x.GetComponent<IHierarchyDivider>()).ToArray();
				
				if (Array.Exists(hds, x => x.GetTransform.childCount > 0))
				{
					if(!EditorUtility.DisplayDialog("Caution: Invalid parenting", "There are dividers with other GameObjects as children. Replacing the dividers will delete these GameObjects.", "Go on", "Abort"))
						return;
				}
				
				int c = hds.Length;
				hcInfo[] _hcInfos = new hcInfo[c];
				
				if (c != 0)
				{
					if(!EditorUtility.DisplayDialog("Replace hierarchy dividers", "This will replace all hierarchy dividers in this scene!", "Go on", "Abort"))
						return;
					
					for (int i = 0; i < c; i++)
					{
						_hcInfos[i] = new hcInfo(hds[i].DividerLabel, hds[i].GetTransform.GetSiblingIndex());
					}
					
					Array.Sort(_hcInfos, delegate(hcInfo _hc1, hcInfo _hc2) {
						return _hc1.Index.CompareTo(_hc2.Index);
					});
				}

				List<HierarchyDivider> _createdHDs = new List<HierarchyDivider>();
				
				int count = dividerPreset.Dividers.Count;
				for (int i = 0; i < count; i++)
				{
					GameObject _go = new GameObject();
					_go.AddComponent(typeof(HierarchyDivider));
					HierarchyDivider hd = _go.GetComponent<HierarchyDivider>();
					hd.SetDivider(dividerPreset.Dividers[i].Label,
					              dividerPreset.Dividers[i].FontColor,
					              dividerPreset.Dividers[i].BGColor,
					              dividerPreset.Dividers[i].OverrideColor_leftSprite,
					              dividerPreset.Dividers[i].OverrideColor_rightSprite,
					              dividerPreset.Dividers[i].BoldFont,
					              dividerPreset.Dividers[i].ItalicFont,
					              dividerPreset.Dividers[i].ColoredFont,
					              dividerPreset.Dividers[i].ColoredBG,
					              dividerPreset.Dividers[i].HasSprite_left,
					              dividerPreset.Dividers[i].HasSprite_right,
					              dividerPreset.Dividers[i].HasOverrideColor_leftSprite,
					              dividerPreset.Dividers[i].HasOverrideColor_rightSprite,
					              dividerPreset.Dividers[i].LeftSprite,
					              dividerPreset.Dividers[i].RightSprite);

					_createdHDs.Add(hd);
				}
				
				int _siblingIndexOffset = 0;
				for (int i = 0; i < c; i++)
				{
					int _match = _createdHDs.FindIndex(x => x.HDV.Label == _hcInfos[i].Label);
					if(_match >= 0)
					{
						_createdHDs[_match].transform.SetSiblingIndex(_hcInfos[i].Index + _siblingIndexOffset);
						_siblingIndexOffset++;
					}
				}
				
				for (int i = 0; i < c; i++)
				{
					DestroyImmediate(hds[i].GetTransform.gameObject);
				}

				EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
			}

			void DeleteAllDividers()
			{
				if(HierarchyDividerDrawing.Dividers == null)
					HierarchyDividerDrawing.Dividers = new List<HierarchyDividerValues>();
				
				MonoBehaviour[] _sceneComponents = FindObjectsOfType<MonoBehaviour>();
				IHierarchyDivider[] hds = _sceneComponents
				                          .Where(x => x.GetComponent<IHierarchyDivider>() != null)
				                          .Select(x => x.GetComponent<IHierarchyDivider>()).ToArray();
				
				if (Array.Exists(hds, x => x.GetTransform.childCount > 0))
				{
					if(!EditorUtility.DisplayDialog("Caution: Invalid parenting", "There are dividers with other GameObjects as children. Replacing the dividers will delete these GameObjects.", "Go on", "Abort"))
						return;
				}
				
				int c = hds.Length;
				if (c != 0)
				{
					if(!EditorUtility.DisplayDialog("Delete hierarchy dividers", "This will delete all hierarchy dividers in this scene!", "Go on", "Abort"))
						return;
					
					for (int i = 0; i < c; i++)
					{
						DestroyImmediate(hds[i].GetTransform.gameObject);
					}
				}
				else
				{
					EditorUtility.DisplayDialog("Delete hierarchy dividers", "No dividers found", "Oh, ok.");
				}
			}
		}
	}
}
#endif