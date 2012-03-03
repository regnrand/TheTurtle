using System.Linq;
using Chorus.merge;
using Chorus.merge.xml.generic;
using FLEx_ChorusPlugin.Infrastructure;
using FLEx_ChorusPlugin.Infrastructure.Handling;
using NUnit.Framework;

namespace FLEx_ChorusPluginTests.Infrastructure.Handling.ReportsByDataType
{
	/// <summary>
	/// Test all expected reports (change and conflict) for the Guid data type.
	///
	/// As of DM 7000052, Guid is used in:
	///
	/// CmPossibilityList	- ListVersion
	/// CmFilter			- App			(immutable)
	/// UserAppFeatAct		- ApplicationId
	/// CmResource			- Version		(immutable)
	/// ScrCheckRun			- CheckId
	/// </summary>
	[TestFixture]
	public class GuidDataTypeReportTests
	{
		private MetadataCache _mdc;
		private XmlMerger _merger;

		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			_mdc = MetadataCache.TestOnlyNewCache;
			_merger = FieldWorksMergeStrategyServices.CreateXmlMergerForFieldWorksData(new NullMergeSituation(), _mdc);
		}

		[Test]
		public void EnsureAllGuidPropertiesAreSetUpCorrectly()
		{
			foreach (var classInfo in _mdc.AllConcreteClasses)
			{
				var clsInfo = classInfo;
				foreach (var elementStrategy in classInfo.AllProperties
					.Where(pi => pi.DataType == DataType.Guid)
					.Select(propertyInfo => _merger.MergeStrategies.ElementStrategies[string.Format("{0}{1}_{2}", propertyInfo.IsCustomProperty ? "Custom_" : "", clsInfo.ClassName, propertyInfo.PropertyName)]))
				{
					Assert.IsFalse(elementStrategy.IsAtomic);
					Assert.IsFalse(elementStrategy.OrderIsRelevant);
					Assert.AreEqual(0, elementStrategy.AttributesToIgnoreForMerging.Count);
					Assert.IsInstanceOf<FindFirstElementWithSameName>(elementStrategy.MergePartnerFinder);
					if (classInfo.ClassName != "CmFilter" && classInfo.ClassName != "CmResource")
					{
						Assert.IsFalse(elementStrategy.IsImmutable);
						continue;
					}
					Assert.IsTrue(elementStrategy.IsImmutable);
				}
			}
		}
	}
}