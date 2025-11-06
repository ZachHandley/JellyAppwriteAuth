import { ref } from 'vue';
import { useApiClient } from './useApiClient';
import { useToast } from './useToast';

export interface EmailActionResponse {
  success: boolean;
  message: string;
}

export function useEmailActions() {
  const apiClient = useApiClient();
  const toast = useToast();

  const loading = ref(false);
  const error = ref<string | null>(null);

  const sendInvite = async (email: string): Promise<void> => {
    if (!email || !email.trim()) {
      toast.error('Email address is required');
      return;
    }

    loading.value = true;
    error.value = null;

    try {
      const response = await apiClient.post<EmailActionResponse>(
        '/Plugin/AppwriteAuth/invite',
        { email: email.trim() }
      );

      if (response.success) {
        toast.success(response.message || 'Invitation sent successfully');
      } else {
        throw new Error(response.message || 'Failed to send invitation');
      }
    } catch (e) {
      const errorMessage = e instanceof Error ? e.message : 'Failed to send invitation';
      error.value = errorMessage;
      toast.error(errorMessage);
      throw e;
    } finally {
      loading.value = false;
    }
  };

  const sendReset = async (email: string): Promise<void> => {
    if (!email || !email.trim()) {
      toast.error('Email address is required');
      return;
    }

    loading.value = true;
    error.value = null;

    try {
      const response = await apiClient.post<EmailActionResponse>(
        '/Plugin/AppwriteAuth/reset',
        { email: email.trim() }
      );

      if (response.success) {
        toast.success(response.message || 'Password reset sent successfully');
      } else {
        throw new Error(response.message || 'Failed to send password reset');
      }
    } catch (e) {
      const errorMessage = e instanceof Error ? e.message : 'Failed to send password reset';
      error.value = errorMessage;
      toast.error(errorMessage);
      throw e;
    } finally {
      loading.value = false;
    }
  };

  const sendTest = async (email: string): Promise<void> => {
    if (!email || !email.trim()) {
      toast.error('Email address is required');
      return;
    }

    loading.value = true;
    error.value = null;

    try {
      const response = await apiClient.post<EmailActionResponse>(
        '/Plugin/AppwriteAuth/test',
        { email: email.trim() }
      );

      if (response.success) {
        toast.success(response.message || 'Test email sent successfully');
      } else {
        throw new Error(response.message || 'Failed to send test email');
      }
    } catch (e) {
      const errorMessage = e instanceof Error ? e.message : 'Failed to send test email';
      error.value = errorMessage;
      toast.error(errorMessage);
      throw e;
    } finally {
      loading.value = false;
    }
  };

  return {
    loading,
    error,
    sendInvite,
    sendReset,
    sendTest,
  };
}
