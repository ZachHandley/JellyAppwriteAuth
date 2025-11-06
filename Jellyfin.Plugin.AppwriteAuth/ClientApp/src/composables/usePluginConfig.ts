import { ref } from 'vue';
import type { PluginConfiguration } from '@/types/config';
import { useApiClient } from './useApiClient';
import { useToast } from './useToast';

const PLUGIN_ID = 'eb5d7894-8eef-4b36-aa6f-5d124e828ce1';

export function usePluginConfig() {
  const apiClient = useApiClient();
  const toast = useToast();

  const config = ref<PluginConfiguration>({
    AppwriteEndpoint: '',
    AppwriteProjectId: '',
    AppwriteApiKey: '',
    EmailProvider: 0,
    SmtpHost: '',
    SmtpPort: 587,
    SmtpUsername: '',
    SmtpPassword: '',
    SmtpFrom: '',
    SmtpEnableSsl: true,
    BrandName: 'Jellyfin',
    LogoUrlOrPath: '',
    BrandPrimaryColor: '#00a4dc',
    LoginUrl: '',
    InviteEmailSubject: 'You have been invited to {{brandName}}',
    InviteEmailHtml: '<p>Welcome to {{brandName}}!</p><p>Your credentials:</p><p>Email: {{email}}</p><p>Password: {{password}}</p><p><a href="{{loginUrl}}">Login here</a></p>',
    ResetEmailSubject: 'Password Reset for {{brandName}}',
    ResetEmailHtml: '<p>Your password has been reset for {{brandName}}.</p><p>New password: {{password}}</p><p><a href="{{loginUrl}}">Login here</a></p>',
    MarkEmailVerifiedOnLogin: true,
  });

  const loading = ref(false);
  const error = ref<string | null>(null);

  const loadConfig = async () => {
    loading.value = true;
    error.value = null;

    try {
      const response = await apiClient.get<PluginConfiguration>(
        `/PluginConfiguration/${PLUGIN_ID}`
      );
      config.value = response;
    } catch (e) {
      const errorMessage = e instanceof Error ? e.message : 'Failed to load configuration';
      error.value = errorMessage;
      toast.error(errorMessage);
      throw e;
    } finally {
      loading.value = false;
    }
  };

  const saveConfig = async () => {
    loading.value = true;
    error.value = null;

    try {
      await apiClient.post<void>(
        `/PluginConfiguration/${PLUGIN_ID}`,
        config.value
      );
      toast.success('Configuration saved successfully');
    } catch (e) {
      const errorMessage = e instanceof Error ? e.message : 'Failed to save configuration';
      error.value = errorMessage;
      toast.error(errorMessage);
      throw e;
    } finally {
      loading.value = false;
    }
  };

  return {
    config,
    loading,
    error,
    loadConfig,
    saveConfig,
  };
}
