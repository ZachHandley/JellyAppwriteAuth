<script setup lang="ts">
import { computed } from 'vue';
import type { PluginConfiguration } from '@/types/config';
import Select from '@/components/ui/Select.vue';
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

const emailProviderOptions = [
  { value: 0, label: 'SMTP' },
  { value: 1, label: 'Appwrite Messaging' },
];

const isSmtpSelected = computed(() => props.modelValue.EmailProvider === 0);

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
  <Card title="Email Provider Configuration">
    <div class="flex flex-col gap-4">
      <Select
        id="email-provider"
        label="Email Provider"
        :model-value="modelValue.EmailProvider"
        :options="emailProviderOptions"
        description="Choose how emails will be sent"
        required
        @update:model-value="updateField('EmailProvider', Number($event))"
      />

      <template v-if="isSmtpSelected">
        <div class="border-t border-gray-200 dark:border-gray-700 pt-4 mt-2">
          <h4 class="text-sm font-semibold text-gray-700 dark:text-gray-300 mb-4">
            SMTP Settings
          </h4>

          <div class="flex flex-col gap-4">
            <TextInput
              id="smtp-host"
              label="SMTP Host"
              :model-value="modelValue.SmtpHost"
              placeholder="smtp.gmail.com"
              description="SMTP server hostname"
              required
              @update:model-value="updateField('SmtpHost', $event as string)"
            />

            <TextInput
              id="smtp-port"
              label="SMTP Port"
              :model-value="modelValue.SmtpPort"
              type="number"
              placeholder="587"
              description="SMTP server port (usually 587 for TLS or 465 for SSL)"
              required
              @update:model-value="updateField('SmtpPort', Number($event))"
            />

            <TextInput
              id="smtp-username"
              label="SMTP Username"
              :model-value="modelValue.SmtpUsername"
              placeholder="your-email@example.com"
              description="Username for SMTP authentication"
              required
              @update:model-value="updateField('SmtpUsername', $event as string)"
            />

            <TextInput
              id="smtp-password"
              label="SMTP Password"
              :model-value="modelValue.SmtpPassword"
              type="password"
              placeholder="your-smtp-password"
              description="Password for SMTP authentication"
              required
              @update:model-value="updateField('SmtpPassword', $event as string)"
            />

            <TextInput
              id="smtp-from"
              label="From Email Address"
              :model-value="modelValue.SmtpFrom"
              type="email"
              placeholder="noreply@example.com"
              description="Email address that will appear as the sender"
              required
              @update:model-value="updateField('SmtpFrom', $event as string)"
            />

            <Checkbox
              id="smtp-enable-ssl"
              label="Enable SSL/TLS"
              :model-value="modelValue.SmtpEnableSsl"
              description="Enable secure connection (recommended)"
              @update:model-value="updateField('SmtpEnableSsl', $event)"
            />
          </div>
        </div>
      </template>

      <div
        v-else
        class="border-t border-gray-200 dark:border-gray-700 pt-4 mt-2"
      >
        <p class="text-sm text-gray-600 dark:text-gray-400">
          Appwrite Messaging will use your Appwrite instance's email configuration.
          Ensure your Appwrite project has email messaging properly configured.
        </p>
      </div>
    </div>
  </Card>
</template>
