using System;
using App.Domain.Clicker;
using NUnit.Framework;

namespace App.Tests.Domain
{
    public sealed class EnergyTests
    {
        [Test]
        public void Full_StartsAtMax()
        {
            var energy = Energy.Full(2000);

            Assert.That(energy.Current, Is.EqualTo(2000));
            Assert.That(energy.Max, Is.EqualTo(2000));
            Assert.That(energy.IsFull, Is.True);
        }

        [Test]
        public void TrySpend_WithEnoughEnergy_DecreasesIt()
        {
            var energy = new Energy(1000, 1000);

            var spent = energy.TrySpend(1);

            Assert.That(spent, Is.True);
            Assert.That(energy.Current, Is.EqualTo(999));
        }

        [Test]
        public void TrySpend_WithoutEnoughEnergy_KeepsItUnchanged()
        {
            var energy = new Energy(1000, 0);

            var spent = energy.TrySpend(1);

            Assert.That(spent, Is.False);
            Assert.That(energy.Current, Is.Zero);
        }

        [Test]
        public void Restore_NeverExceedsMax()
        {
            var energy = new Energy(1000, 995);

            var restored = energy.Restore(10);

            Assert.That(restored, Is.EqualTo(5));
            Assert.That(energy.Current, Is.EqualTo(1000));
            Assert.That(energy.IsFull, Is.True);
        }

        [Test]
        public void Changes_RaiseChangedEvent_WithNewValue()
        {
            var energy = new Energy(10, 10);
            var reported = -1;
            energy.Changed += value => reported = value;

            energy.TrySpend(3);

            Assert.That(reported, Is.EqualTo(7));
        }

        [Test]
        public void RestoringFullEnergy_DoesNotRaiseChanged()
        {
            var energy = new Energy(10, 10);
            var raised = false;
            energy.Changed += _ => raised = true;

            energy.Restore(10);

            Assert.That(raised, Is.False);
        }

        [Test]
        public void InitialValueAboveMax_Throws() =>
            Assert.Throws<ArgumentOutOfRangeException>(() => new Energy(10, 11));
    }
}
