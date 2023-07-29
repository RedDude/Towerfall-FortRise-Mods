using System.Collections.Generic;

namespace TowerFall
{
	public class VariantPickupController
	{
		public MatchVariants matchVariants;

		public static Dictionary<Player, Variant> itemVariantsPerPlayer = new Dictionary<Player, Variant>();
		
		public static Dictionary<Variant, bool> keepAfterDead = new Dictionary<Variant, bool>();
		private VariantPickupController()
		{
			// matchVariants = new MatchVariants(true);
		}
		
		private static VariantPickupController _instance;

		public static VariantPickupController GetInstance()
		{
			return _instance ??= new VariantPickupController();
		}
	}
}
