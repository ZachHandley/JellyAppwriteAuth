<script setup lang="ts">
import type { PluginConfiguration } from '@/types/config';
import TextInput from '@/components/ui/TextInput.vue';
import Checkbox from '@/components/ui/Checkbox.vue';
import Card from '@/components/ui/Card.vue';

interface Props {
  modelValue: PluginConfiguration;
}

const props = defineProps<Props>();

const emit = defineEmits<{
  'update:modelValue': [value: PluginConfiguration];
}>();

const updateField = <K extends keyof PluginConfiguration>(
  field: K,
  value: PluginConfiguration[K]
) => {
  emit('update:modelValue', {
    ...props.modelValue,
    [field]: value,
  });
};
</script>

<template>
  <Card title="Appwrite Configuration">
    <div class="flex flex-col gap-4">
      <TextInput
        id="appwrite-endpoint"
        label="Appwrite Endpoint"
        :model-value="modelValue.AppwriteEndpoint"
        type="url"
        placeholder="https://cloud.appwrite.io/v1"
        description="The URL of your Appwrite instance"
        required
        @update:model-value="updateField('AppwriteEndpoint', $event as string)"
      />

      <TextInput
        id="appwrite-project-id"
        label="Appwrite Project ID"
        :model-value="modelValue.AppwriteProjectId"
        placeholder="your-project-id"
        description="The unique identifier for your Appwrite project"
        required
        @update:model-value="updateField('AppwriteProjectId', $event as string)"
      />

      <TextInput
        id="appwrite-api-key"
        label="Appwrite API Key"
        :model-value="modelValue.AppwriteApiKey || ''"
        type="password"
        placeholder="your-api-key"
        description="API key with appropriate permissions (users.read, users.write, optional)"
        @update:model-value="updateField('AppwriteApiKey', $event as string)"
      />

      <Checkbox
        id="mark-email-verified"
        label="Mark Email as Verified on Login"
        :model-value="modelValue.MarkEmailVerifiedOnLogin"
        description="Automatically mark user emails as verified when they successfully login (requires API key)"
        @update:model-value="updateField('MarkEmailVerifiedOnLogin', $event)"
      />
    </div>
  </Card>
</template>
