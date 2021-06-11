using System.Collections.Generic;
using ZooLab.BusinessLogic.Exceptions;

namespace ZooLab.BusinessLogic.Zoos
{
	public class ResultItemListByObject<TObject, TItem>
	{
		#region Public Properties

		public bool IsEmpty => Data.Count == 0;

		private string _reasonEmpty;
		/// <summary>
		/// The message describing the reason the result data is empty.
		/// <br/>
		/// Should not be used when the result is not empty (check with <see cref="IsEmpty"/>).
		/// </summary>
		public string ReasonEmpty
		{
			get => _reasonEmpty ?? "Reason not specified.";
			set
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					throw new AssignNullOrEmptyException(nameof(ReasonEmpty), noWhitespace: true);
				}
				_reasonEmpty = value;
			}
		}

		private Dictionary<TObject, List<TItem>> _data;
		/// <summary>
		/// Holds the desired result (empty on the instance creation).
		/// Cannot be null.
		/// </summary>
		public Dictionary<TObject, List<TItem>> Data
		{
			get => _data;
			private set
			{
				_data = value ?? throw new AssignNullException(nameof(Data));
			}
		}

		#endregion

		#region Constructor

		/// <summary>
		/// Default constructor.
		/// </summary>
		public ResultItemListByObject()
		{
			Data = new();
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Adds the specified item to the list of items belonging to the specified object.
		/// </summary>
		/// <param name="obj">The object to add the item for.</param>
		/// <param name="item">The item to add.</param>
		public void AddItemForObject(TObject obj, TItem item)
		{
			if (Data.ContainsKey(obj))
			{
				Data[obj].Add(item);
			}
			else
			{
				Data.Add(obj, new() { item });
			}
		}

		#endregion
	}
}
