#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace ManasparkAssets
{
	namespace HierarchyDividers
	{
		[InitializeOnLoad]
		[Serializable]
		public class HierarchyDividerDrawing
		{
			private static Material spriteMatTransparent;
			
			[SerializeField]
			public static List<HierarchyDividerValues> Dividers;
			
			private static Vector2 offset = new Vector2(0, 2);

			static HierarchyDividerDrawing()
			{
				LoadMaterials();
				EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchyWindowItemOnGUI;
			}

			public static void LoadMaterials()
			{
				spriteMatTransparent = Resources.Load<Material>("Basic/Material/EditorSpriteTransparent");
			}

			private static void HandleHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
			{
                if(spriteMatTransparent == null)
					LoadMaterials();

				GameObject obj = (GameObject) EditorUtility.InstanceIDToObject(instanceID);

				if(obj == null)
					return;

				HierarchyDivider found = obj.GetComponent<HierarchyDivider>();
				if(found == null)
					return;
				
				HierarchyDividerValues foundDivider = found.HDV;
				
				if (Dividers == null || Dividers.Count == 0)
				{
					Dividers = new List<HierarchyDividerValues>();
					Dividers.AddRange(GameObject.FindObjectsOfType<HierarchyDivider>().ToList().Select(x => x.HDV).ToList());
				}

				try
				{
					HierarchyDividerValues divider = Dividers.Find(x => x.DividerID == foundDivider.DividerID);
					if (divider == null)
					{
						Dividers.AddRange(GameObject.FindObjectsOfType<HierarchyDivider>().ToList().Select(x => x.HDV).ToList());
					}
					
					if (obj != null && divider != null)
					{
						
						Color fontColor = divider.fontColor;
						Color bgColor = new Color(divider.bgColor.r, divider.bgColor.g, divider.bgColor.b, 1f);
					
						Color standardBg = EditorGUIUtility.isProSkin ?
							new Color(0.22f, 0.22f, 0.22f)
							: new Color(.76f, .76f, .76f);
						
						Color _selectionColor = EditorGUIUtility.isProSkin ? new Color(0.24f, 0.37f, 0.59f) : new Color(0.24f, 0.48f, 0.90f);

						bool selected = Selection.instanceIDs.Contains(instanceID);
						if (selected)
						{
							fontColor = Color.white;
							bgColor = _selectionColor;
						}

						float spriteSize = 15f;
					
						#if UNITY_2019_2
						selectionRect.x += 18;
						selectionRect.width -= 4;
						#elif UNITY_2019_1_OR_NEWER
						selectionRect.x += 20;
						selectionRect.width -= 6;
						#endif
						
						Rect offsetRect = new Rect(selectionRect.position + offset, selectionRect.size);
						
						#if UNITY_2018_3
						offsetRect.width += 15f;
						#endif
						
						if (divider.hasBgColor || selected)
						{
							if (divider.hasRightSprite)
							{
								#if UNITY_2019_1_OR_NEWER
								EditorGUI.DrawRect(new Rect(selectionRect.x - 2, selectionRect.y, offsetRect.width - (spriteSize + 4) - 5 - 8 - 16, offsetRect.height), bgColor);
								#else
								EditorGUI.DrawRect(new Rect(selectionRect.x, selectionRect.y, offsetRect.width - (spriteSize + 4), offsetRect.height), bgColor);
								#endif
							}
							else
							{
								{
									#if UNITY_2019_2
									EditorGUI.DrawRect(new Rect(selectionRect.x, selectionRect.y, offsetRect.width + 2, offsetRect.height), bgColor);
									#else
									EditorGUI.DrawRect(new Rect(selectionRect.x - 2, selectionRect.y, offsetRect.width - 16, offsetRect.height), bgColor);
									#endif
								}

							}
						}
						else
						{
							#if UNITY_2019_2
							EditorGUI.DrawRect(new Rect(selectionRect.x - 45, selectionRect.y, offsetRect.width + 45, selectionRect.height), standardBg);
							#else
 							EditorGUI.DrawRect(new Rect(selectionRect.x, selectionRect.y, offsetRect.width, selectionRect.height), standardBg);
							#endif
						}

					
						GUIStyle _style = new GUIStyle();
						if (!divider.boldFont && !divider.italicFont)
							_style.fontStyle = FontStyle.Normal;
						if (divider.boldFont && !divider.italicFont)
							_style.fontStyle = FontStyle.Bold;
						else if(!divider.boldFont && divider.italicFont)
							_style.fontStyle = FontStyle.Italic;
						else if(divider.boldFont && divider.italicFont)
							_style.fontStyle = FontStyle.BoldAndItalic;

						_style.normal = divider.hasFontColor || selected
							? new GUIStyleState() {textColor = fontColor}
							: new GUIStyleState();
						
						_style.alignment = TextAnchor.UpperCenter;
						
//						if (divider.hasRightSprite)
//							EditorGUI.LabelField(new Rect(offsetRect.x, offsetRect.y, offsetRect.width - (spriteSize + 4f), offsetRect.height), obj.name, _style);
//						else
//							EditorGUI.LabelField(offsetRect, obj.name, _style);
						EditorGUI.LabelField(new Rect(offsetRect.x, offsetRect.y, offsetRect.width - (spriteSize + 4f), offsetRect.height), obj.name, _style);
						
						#if UNITY_2019_2
						EditorGUI.DrawRect(new Rect(16f, offsetRect.y - 2, 45, EditorGUIUtility.singleLineHeight), selected? _selectionColor : standardBg);
						#elif UNITY_2019_1_OR_NEWER
						EditorGUI.DrawRect(new Rect(25f, offsetRect.y - 2, 30, EditorGUIUtility.singleLineHeight), selected? _selectionColor : standardBg);
						#else
						EditorGUI.DrawRect(new Rect(0f, offsetRect.y - 2, 30, EditorGUIUtility.singleLineHeight), selected? _selectionColor : standardBg);
						#endif
						
						if (divider.hasLeftSprite)
						{
							Rect _leftSpriteRect = new Rect(offsetRect.x - (spriteSize * 1.5f), offsetRect.y - 2,
							                                spriteSize, EditorGUIUtility.singleLineHeight);

							#if UNITY_2019_2
							_leftSpriteRect.x += 4;
							#elif UNITY_2019_1_OR_NEWER
							_leftSpriteRect.x += 3;
							#endif

							DrawSprite(_leftSpriteRect,
							           divider.leftSprite, spriteSize);
						}

						if (divider.hasRightSprite)
						{
							Rect _rightSpriteRect = new Rect(offsetRect.x + offsetRect.width - spriteSize + 2,
							                                 offsetRect.y - 2, spriteSize,
							                                 EditorGUIUtility.singleLineHeight);
							
							#if UNITY_2019_1_OR_NEWER
							EditorGUI.DrawRect(new Rect(_rightSpriteRect.x - 5f, _rightSpriteRect.y, _rightSpriteRect.width + 55f, _rightSpriteRect.height), selected? _selectionColor : standardBg);
							_rightSpriteRect.x -= 2;
							#else
							EditorGUI.DrawRect(new Rect(_rightSpriteRect.x - 1f, _rightSpriteRect.y, _rightSpriteRect.width + 50f, _rightSpriteRect.height), selected? _selectionColor : standardBg);
							#endif
							
							DrawSprite(_rightSpriteRect,
							           divider.rightSprite, spriteSize);
						}
					}
				}
				catch (Exception e)
				{
					throw e;
				}				
			}

			public static void DrawSprite(Rect rect, Texture _tex, float _size)
			{
				calculateIconRect(ref rect, _size);

				EditorGUI.DrawPreviewTexture(rect, _tex, spriteMatTransparent, ScaleMode.ScaleToFit, 0f);
			}

			private static void calculateIconRect(ref Rect _rect, float _size)
			{
				_rect.x += 0.5f * EditorGUIUtility.singleLineHeight - 0.5f * _size;
				_rect.y += 0.5f * EditorGUIUtility.singleLineHeight - 0.5f * _size;
				
				_rect.width = _rect.height = _size;
			}
		}
	}
}
#endif