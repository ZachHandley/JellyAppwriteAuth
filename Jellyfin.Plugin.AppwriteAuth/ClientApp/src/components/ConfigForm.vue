<script setup lang="ts">
import { onMounted } from 'vue';
import { usePluginConfig } from '@/composables/usePluginConfig';
import AppwriteSection from '@/components/sections/AppwriteSection.vue';
import EmailProviderSection from '@/components/sections/EmailProviderSection.vue';
import BrandingSection from '@/components/sections/BrandingSection.vue';
import EmailTemplatesSection from '@/components/sections/EmailTemplatesSection.vue';
import AdminActionsSection from '@/components/sections/AdminActionsSection.vue';
import Button from '@/components/ui/Button.vue';

const { config, loading, loadConfig, saveConfig } = usePluginConfig();

onMounted(async () => {
  try {
    await loadConfig();
  } catch (error) {
    console.error('Failed to load configuration on mount:', error);
  }
});

const handleSaveConfig = async () => {
  try {
    await saveConfig();
  } catch (error) {
    console.error('Failed to save configuration:', error);
  }
};
</script>

<template>
  <div class="max-w-4xl mx-auto px-4 py-8">
    <div class="mb-8">
      <h1 class="text-3xl font-bold text-gray-900 dark:text-gray-100 mb-2">
        Appwrite Authentication Configuration
      </h1>
      <p class="text-gray-600 dark:text-gray-400">
        Configure Appwrite authentication, email settings, and user management for your Jellyfin server.
      </p>
    </div>

    <div v-if="loading && !config.AppwriteEndpoint" class="flex justify-center items-center py-12">
      <svg
        class="animate-spin h-8 w-8 text-blue-600 dark:text-blue-400"
        xmlns="http://www.w3.org/2000/svg"
        fill="none"
        viewBox="0 0 24 24"
      >
        <circle
          class="opacity-25"
          cx="12"
          cy="12"
          r="10"
          stroke="currentColor"
          stroke-width="4"
        ></circle>
        <path
          class="opacity-75"
          fill="currentColor"
          d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
        ></path>
      </svg>
    </div>

    <div v-else class="flex flex-col gap-6">
      <AppwriteSection v-model="config" />

      <EmailProviderSection v-model="config" />

      <BrandingSection v-model="config" />

      <EmailTemplatesSection v-model="config" />

      <AdminActionsSection />

      <div class="sticky bottom-4 bg-white dark:bg-gray-800 rounded-lg shadow-lg border border-gray-200 dark:border-gray-700 p-4">
        <div class="flex justify-end gap-3">
          <Button
            variant="primary"
            :loading="loading"
            @click="handleSaveConfig"
          >
            Save Configuration
          </Button>
        </div>
      </div>
    </div>
  </div>
</template>
