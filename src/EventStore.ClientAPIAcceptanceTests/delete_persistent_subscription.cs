using System;
using System.Threading.Tasks;
using EventStore.ClientAPI.Exceptions;
using Xunit;

namespace EventStore.ClientAPI.Tests {
	public class delete_persistent_subscription : EventStoreClientAPITest {
		private const string Group = nameof(delete_persistent_subscription);
		private readonly EventStoreClientAPIFixture _fixture;

		public delete_persistent_subscription(EventStoreClientAPIFixture fixture) {
			_fixture = fixture;
		}

		[Theory, MemberData(nameof(UseSslTestCases))]
		public async Task without_credentials_fails(bool useSsl) {
			var streamName = $"{GetStreamName()}_{useSsl}";
			var connection = _fixture.Connections[useSsl];

			await connection.CreatePersistentSubscriptionAsync(streamName, Group,
				PersistentSubscriptionSettings.Create(), DefaultUserCredentials.Admin).WithTimeout();

			await Assert.ThrowsAsync<AccessDeniedException>(
				() => connection.DeletePersistentSubscriptionAsync(streamName, Group).WithTimeout());
		}

		[Theory, MemberData(nameof(UseSslTestCases))]
		public async Task that_does_not_exist_fails(bool useSsl) {
			var streamName = $"{GetStreamName()}_{useSsl}";
			var connection = _fixture.Connections[useSsl];

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => connection.DeletePersistentSubscriptionAsync(streamName, Group, DefaultUserCredentials.Admin)
					.WithTimeout());
		}

		[Theory, MemberData(nameof(UseSslTestCases))]
		public async Task with_credentials_succeeds(bool useSsl) {
			var streamName = $"{GetStreamName()}_{useSsl}";
			var connection = _fixture.Connections[useSsl];

			await connection.CreatePersistentSubscriptionAsync(streamName, Group,
				PersistentSubscriptionSettings.Create(), DefaultUserCredentials.Admin).WithTimeout();

			await connection.DeletePersistentSubscriptionAsync(streamName, Group, DefaultUserCredentials.Admin)
				.WithTimeout();
		}
	}
}
