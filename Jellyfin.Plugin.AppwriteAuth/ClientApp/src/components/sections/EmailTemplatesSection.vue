<script setup lang="ts">
import type { PluginConfiguration } from '@/types/config';
import TextInput from '@/components/ui/TextInput.vue';
import TextArea from '@/components/ui/TextArea.vue';
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

const templateVars = [
  { code: '{{brandName}}', description: 'Your brand name' },
  { code: '{{email}}', description: "User's email address" },
  { code: '{{password}}', description: 'Generated password (invite/reset only)' },
  { code: '{{loginUrl}}', description: 'Login page URL' },
  { code: '{{logo}}', description: 'Logo URL' },
];
</script>

<template>
  <Card title="Email Templates">
    <div class="flex flex-col gap-6">
      <div class="bg-blue-50 dark:bg-blue-900/20 border border-blue-200 dark:border-blue-800 rounded-md px-4 py-3">
        <p class="text-sm text-blue-800 dark:text-blue-200 font-medium mb-2">
          Available Template Variables
        </p>
        <ul class="text-xs text-blue-700 dark:text-blue-300 space-y-1">
          <li v-for="(varItem, index) in templateVars" :key="index">
            <code class="bg-blue-100 dark:bg-blue-900/40 px-1 py-0.5 rounded">{{ varItem.code }}</code> - {{ varItem.description }}
          </li>
        </ul>
      </div>

      <div class="border-t border-gray-200 dark:border-gray-700 pt-4">
        <h4 class="text-sm font-semibold text-gray-700 dark:text-gray-300 mb-4">
          Invitation Email
        </h4>

        <div class="flex flex-col gap-4">
          <TextInput
            id="invite-subject"
            label="Subject"
            :model-value="modelValue.InviteEmailSubject"
            placeholder="You have been invited to {{brandName}}"
            @update:model-value="updateField('InviteEmailSubject', $event as string)"
          />

          <TextArea
            id="invite-html"
            label="HTML Body"
            :model-value="modelValue.InviteEmailHtml"
            :rows="6"
            placeholder="<p>Welcome to {{brandName}}!</p>"
            description="HTML content for invitation emails"
            @update:model-value="updateField('InviteEmailHtml', $event)"
          />
        </div>
      </div>

      <div class="border-t border-gray-200 dark:border-gray-700 pt-4">
        <h4 class="text-sm font-semibold text-gray-700 dark:text-gray-300 mb-4">
          Password Reset Email
        </h4>

        <div class="flex flex-col gap-4">
          <TextInput
            id="reset-subject"
            label="Subject"
            :model-value="modelValue.ResetEmailSubject"
            placeholder="Password Reset for {{brandName}}"
            @update:model-value="updateField('ResetEmailSubject', $event as string)"
          />

          <TextArea
            id="reset-html"
            label="HTML Body"
            :model-value="modelValue.ResetEmailHtml"
            :rows="6"
            placeholder="<p>Your password has been reset.</p>"
            description="HTML content for password reset emails"
            @update:model-value="updateField('ResetEmailHtml', $event)"
          />
        </div>
      </div>
    </div>
  </Card>
</template>
