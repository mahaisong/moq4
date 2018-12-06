// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

namespace Moq.Tests
{
	public class ExtensibilityFixture
	{
		[Fact]
		public void ShouldExtendMatching()
		{
			var mock = new Mock<IOrderRepository>();
			mock.Setup(repo => repo.Save(Order.IsBig()))
				.Throws(new InvalidOperationException());

			try
			{
				mock.Object.Save(new Order { Amount = 1000 });

				Assert.True(false, "Should have failed for big order");
			}
			catch (InvalidOperationException)
			{
			}
		}

		[Fact]
		public void ShouldExtendWithSimpleMatchers()
		{
			var order = new Order();
			var mock = new Mock<IOrderRepository>();

			Order repo = Match.Create<Order>(r => true);

			mock.Setup(x => x.Save(Orders.Contains(order)))
				 .Throws<ArgumentException>();

			Assert.Throws<ArgumentException>(() => mock.Object.Save(new[] { order }));
		}

		[Fact]
		public void ShouldExtendWithPropertyMatchers()
		{
			var mock = new Mock<IOrderRepository>();
			mock.Setup(repo => repo.Save(Order.IsSmall))
				.Throws(new InvalidOperationException());

			try
			{
				mock.Object.Save(new Order { Amount = 50 });

				Assert.True(false, "Should have failed for small order");
			}
			catch (InvalidOperationException)
			{
			}
		}

		[Fact]
		public void SetterMatcherRendersNicely()
		{
			var mock = new Mock<IOrderRepository>();

			var ex = Record.Exception(() => mock.VerifySet(repo => repo.Value = It.IsAny<int>()));
			Assert.Contains("repo => repo.Value = It.IsAny<int>()", ex.Message);
		}
	}

	public static class Orders
	{
		[Matcher]
		public static IEnumerable<Order> Contains(Order order)
		{
			return Match.Create<IEnumerable<Order>>(orders => orders.Contains(order));
		}
	}

	public interface IOrderRepository
	{
		void Save(Order order);
		void Save(IEnumerable<Order> orders);
		int Value { get; set; }
	}

	public class Order
	{
		public int Amount { get; set; }

		[Matcher]
		public static Order IsBig()
		{
			return Match.Create<Order>(o => o.Amount >= 1000);
		}


		[Matcher]
		public static Order IsSmall => Match.Create<Order>(o => o.Amount <= 1000);
	}
}
