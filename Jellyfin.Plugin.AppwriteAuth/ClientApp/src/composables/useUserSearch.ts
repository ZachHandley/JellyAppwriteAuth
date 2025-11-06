import { ref, computed } from 'vue';
import { useDebounceFn } from '@vueuse/core';
import { useApiClient } from './useApiClient';

export interface JellyfinUser {
  Id: string;
  Name: string;
  ServerId?: string;
  HasPassword?: boolean;
  HasConfiguredPassword?: boolean;
  HasConfiguredEasyPassword?: boolean;
  EnableAutoLogin?: boolean;
  LastLoginDate?: string;
  LastActivityDate?: string;
  Policy?: {
    IsAdministrator: boolean;
    IsHidden: boolean;
    IsDisabled: boolean;
  };
}

export interface ResolveEmailResponse {
  email: string;
  userName: string;
}

export function useUserSearch() {
  const apiClient = useApiClient();

  const users = ref<JellyfinUser[]>([]);
  const searchQuery = ref('');
  const loading = ref(false);
  const error = ref<string | null>(null);

  const filteredUsers = computed(() => {
    if (!searchQuery.value.trim()) {
      return users.value;
    }

    const query = searchQuery.value.toLowerCase();
    return users.value.filter((user) =>
      user.Name.toLowerCase().includes(query)
    );
  });

  const loadUsers = async () => {
    loading.value = true;
    error.value = null;

    try {
      const response = await apiClient.get<{ Items: JellyfinUser[] }>('/Users');
      users.value = response.Items || [];
    } catch (e) {
      const errorMessage = e instanceof Error ? e.message : 'Failed to load users';
      error.value = errorMessage;
      throw e;
    } finally {
      loading.value = false;
    }
  };

  const resolveEmail = async (userName: string): Promise<string> => {
    if (!userName || !userName.trim()) {
      throw new Error('Username is required');
    }

    try {
      const response = await apiClient.get<ResolveEmailResponse>(
        `/Plugin/AppwriteAuth/resolve-email?userName=${encodeURIComponent(userName)}`
      );
      return response.email;
    } catch (e) {
      const errorMessage = e instanceof Error ? e.message : 'Failed to resolve email';
      throw new Error(errorMessage);
    }
  };

  // Debounced search function (300ms delay)
  const debouncedSearch = useDebounceFn((query: string) => {
    searchQuery.value = query;
  }, 300);

  const setSearchQuery = (query: string) => {
    debouncedSearch(query);
  };

  return {
    users,
    searchQuery,
    filteredUsers,
    loading,
    error,
    loadUsers,
    resolveEmail,
    setSearchQuery,
  };
}
