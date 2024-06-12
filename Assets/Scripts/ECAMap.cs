// 生成时间：2024/6/12 17:44:38
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ECA
{
	public static class ECAMap
	{
		public static readonly Dictionary<string, MethodInfo> sMethods = new Dictionary<string, MethodInfo>()
		{
			{"AdjEscape", typeof(Brawl.AI.Chatbot).GetMethod(nameof(Brawl.AI.Chatbot.AdjEscape), new Type[] {typeof(System.Single),typeof(System.Single)})},
			{"AdjPatrol", typeof(Brawl.AI.Chatbot).GetMethod(nameof(Brawl.AI.Chatbot.AdjPatrol), new Type[] {typeof(System.Single),typeof(System.Single)})},
			{"ChangeState", typeof(Brawl.AI.Chatbot).GetMethod(nameof(Brawl.AI.Chatbot.ChangeState), new Type[] {typeof(System.String)})},
			{"CreateState", typeof(Brawl.AI.Chatbot).GetMethod(nameof(Brawl.AI.Chatbot.CreateState), new Type[] {typeof(System.String),typeof(System.String)})},
		};
	}
}