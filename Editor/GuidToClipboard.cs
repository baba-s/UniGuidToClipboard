using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace UniGuidToClipboard
{
	/// <summary>
	/// すべてのアセットの GUID をクリップボードにコピーするエディタ拡張
	/// </summary>
	public static class GuidToClipboard
	{
		//================================================================================
		// クラス
		//================================================================================
		public sealed class Data
		{
			public string Path { get; }
			public string Guid { get; }

			internal Data( string path, string guid )
			{
				Path = path;
				Guid = guid;
			}
		}

		//================================================================================
		// デリゲート（static）
		//================================================================================
		/// <summary>
		/// コピーする時に実行する処理を差し替えたい場合はこのデリゲートにコールバックを登録します
		/// </summary>
		public static Action<IEnumerable<Data>> OnCopy { private get; set; }

		//================================================================================
		// 関数（static）
		//================================================================================
		/// <summary>
		/// Unity メニューからコピーする時に呼び出される関数
		/// </summary>
		[MenuItem( "Assets/UniGuidToClipboard/すべてのアセットの GUID をクリップボードにコピー" )]
		private static void Copy()
		{
			var list = GetList();

			if ( OnCopy != null )
			{
				OnCopy( list );
				return;
			}

			var result = string.Join( "\n", list.Select( c => $"{c.Path},{c.Guid}" ) );

			EditorGUIUtility.systemCopyBuffer = result;

			EditorUtility.DisplayDialog
			(
				title: "UniGuidToClipboard",
				message: "すべてのアセットの GUID をクリップボードにコピーしました",
				ok: "OK"
			);
		}

		/// <summary>
		/// アセットのパスと GUID の一覧を返します
		/// </summary>
		public static IEnumerable<Data> GetList()
		{
			return AssetDatabase
					.GetAllAssetPaths()
					.Select( c => new Data( c, AssetDatabase.AssetPathToGUID( c ) ) )
					.OrderBy( c => c.Path )
					.ToArray()
				;
		}
	}
}