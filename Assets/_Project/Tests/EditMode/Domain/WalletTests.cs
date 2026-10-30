using System;
using App.Domain.Clicker;
using NUnit.Framework;

namespace App.Tests.Domain
{
    public sealed class WalletTests
    {
        [Test]
        public void Add_IncreasesBalance_AndRaisesBalanceChanged()
        {
            var wallet = new Wallet(5);
            var reported = 0;
            wallet.BalanceChanged += value => reported = value;

            wallet.Add(1);

            Assert.That(wallet.Balance, Is.EqualTo(6));
            Assert.That(reported, Is.EqualTo(6));
        }

        [Test]
        public void AddNegativeAmount_Throws() =>
            Assert.Throws<ArgumentOutOfRangeException>(() => new Wallet(0).Add(-1));
    }
}
