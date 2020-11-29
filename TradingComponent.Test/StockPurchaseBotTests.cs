using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using TradingApi;

namespace TradingComponent.Test
{
    [TestFixture]
    public class TradingComponentsTest
    {
        Mock<IBackOffice> mockBackOffice;

        [SetUp]
        public void Setup()
        {
            // create stub
            mockBackOffice = new Mock<IBackOffice>();
            mockBackOffice.Setup(m => m.BuyStock("FB", 100, 199.9m));
            mockBackOffice.Setup(m => m.BuyStock("GOOG", 200, 159.9m)).Throws(new Exception("No eough stock available"));
            mockBackOffice.Setup(m => m.BuyStock("MSFT", 300, 99.9m));
        }

        [Test]
        [TestCase]
        public void Should_Buy_BlowPrice_SingleBot()
        {
            // Arrange
            var bot = new StockPurchaseBot("FB", 100, 200.0m)
            {
                BackOffice = mockBackOffice.Object
            };

            var argsList = new List<CompletionEventArgs>();
            bot.CompletionEvent += delegate (object sender, CompletionEventArgs args)
            {
                argsList.Add(args);
            };

            // Act
            bot.ReceivePriceTick("FB", 199.9m);

            // Assert
            mockBackOffice.Verify();
            Assert.AreEqual(1, argsList.Count);
            Assert.AreEqual("FB", argsList[0].Symbol);
            Assert.AreEqual(100, argsList[0].Quantity);
            Assert.AreEqual(199.9m, argsList[0].MarketPrice);
        }

        [Test]
        [TestCase]
        public void Should_NotBuy_AbovePrice_SingleBot()
        {
            // Arrange
            var bot = new StockPurchaseBot("FB", 100, 200.0m)
            {
                BackOffice = mockBackOffice.Object
            };

            var argsList = new List<CompletionEventArgs>();
            bot.CompletionEvent += delegate (object sender, CompletionEventArgs args)
            {
                argsList.Add(args);
            };

            // Act
            bot.ReceivePriceTick("FB", 199.9m);

            // Assert
            mockBackOffice.Verify();
            Assert.AreEqual(1, argsList.Count);
            Assert.AreEqual("FB", argsList[0].Symbol);
            Assert.AreEqual(100, argsList[0].Quantity);
            Assert.AreEqual(199.9m, argsList[0].MarketPrice);
        }

        [Test]
        [TestCase]
        public void Should_NotBuy_OtherStock_SingleBot()
        {
            // Arrange
            var bot = new StockPurchaseBot("FB", 100, 200.0m)
            {
                BackOffice = mockBackOffice.Object
            };

            var argsList = new List<CompletionEventArgs>();
            bot.CompletionEvent += delegate (object sender, CompletionEventArgs args)
            {
                argsList.Add(args);
            };

            // Act
            bot.ReceivePriceTick("GOOG", 1200.0m);

            // Assert
            mockBackOffice.Verify();
            Assert.AreEqual(0, argsList.Count);
        }

        [Test]
        [TestCase]
        public void Should_Buy_BlowPrice_SingleBot_ManyStocks()
        {
            // Arrange
            var bot = new StockPurchaseBot("FB", 100, 200.0m)
            {
                BackOffice = mockBackOffice.Object
            };

            var argsList = new List<CompletionEventArgs>();
            bot.CompletionEvent += delegate (object sender, CompletionEventArgs args)
            {
                argsList.Add(args);
            };

            // Act
            bot.ReceivePriceTick("FB", 199.9m);
            bot.ReceivePriceTick("MSFT", 1000m);

            // Assert
            mockBackOffice.Verify();
            Assert.AreEqual(1, argsList.Count);
            Assert.AreEqual("FB", argsList[0].Symbol);
            Assert.AreEqual(100, argsList[0].Quantity);
            Assert.AreEqual(199.9m, argsList[0].MarketPrice);
        }

        [Test]
        [TestCase]
        public void Should_Buy_BlowPrice_ManyBots()
        {
            // Arrange
            var bots = new List<StockPurchaseBot>();
            bots.Add(new StockPurchaseBot("FB", 100, 200.0m)
            {
                BackOffice = mockBackOffice.Object
            });

            bots.Add(new StockPurchaseBot("MSFT", 300, 100.0m)
            {
                BackOffice = mockBackOffice.Object
            });

            var argsList = new List<CompletionEventArgs>();
            foreach (var bot in bots)
            {
                bot.CompletionEvent += delegate (object sender, CompletionEventArgs args)
                {
                    argsList.Add(args);
                };
            }

            // Act
            foreach (var bot in bots)
            {
                bot.ReceivePriceTick("FB", 199.9m);
                bot.ReceivePriceTick("MSFT", 99.0m);
                bot.ReceivePriceTick("GOOG", 999.0m);
            }

            // Assert
            mockBackOffice.Verify();
            Assert.AreEqual(2, argsList.Count);
            Assert.AreEqual("FB", argsList[0].Symbol);
            Assert.AreEqual(100, argsList[0].Quantity);
            Assert.AreEqual(199.9m, argsList[0].MarketPrice);

            Assert.AreEqual("MSFT", argsList[1].Symbol);
            Assert.AreEqual(300, argsList[1].Quantity);
            Assert.AreEqual(99.0m, argsList[1].MarketPrice);
        }

        [Test]
        [TestCase]
        public void Should_Throw_Exception_When_BackOfficeIsNull()
        {
            // Arrange
            var bot = new StockPurchaseBot("GOOG", 200, 160.0m);

            var errorList = new List<FailureEventArgs>();
            bot.FailureEvent += delegate (object sender, FailureEventArgs error) {
                errorList.Add(error);
            };

            // Act
            bot.ReceivePriceTick("GOOG", 159.9m);

            // Assert
            mockBackOffice.Verify();
            Assert.AreEqual(1, errorList.Count);
            Assert.AreEqual("BackOffice is not initialized", errorList[0].Error);
        }

        [Test]
        [TestCase]
        public void Should_Throw_Exception_When_NoEnoughStock()
        {
            // Arrange
            var bot = new StockPurchaseBot("GOOG", 200, 160.0m)
            {
                BackOffice = mockBackOffice.Object
            };

            var argsList = new List<CompletionEventArgs>();
            bot.CompletionEvent += delegate (object sender, CompletionEventArgs args)
            {
                argsList.Add(args);
            };

            var errorList = new List<FailureEventArgs>();
            bot.FailureEvent += delegate (object sender, FailureEventArgs error) {
                errorList.Add(error);
            };

            // Act
            bot.ReceivePriceTick("GOOG", 159.9m);

            // Assert
            mockBackOffice.Verify();
            Assert.AreEqual(0, argsList.Count);
            Assert.AreEqual(1, errorList.Count);
            Assert.AreEqual("GOOG", errorList[0].Symbol);
            Assert.AreEqual(200, errorList[0].Quantity);
            Assert.AreEqual(159.9m, errorList[0].MarketPrice);
            Assert.AreEqual("No eough stock available", errorList[0].Error);
        }
    }
}