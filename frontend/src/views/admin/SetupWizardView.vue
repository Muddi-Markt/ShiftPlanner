<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import { Check, Plus, X } from '@lucide/vue'
import { toast } from 'vue-sonner'
import PageHeader from '@/components/PageHeader.vue'
import FormField from '@/components/FormField.vue'
import ColorField from '@/components/ColorField.vue'
import DurationField from '@/components/DurationField.vue'
import { Button } from '@/components/ui/button'
import { Checkbox } from '@/components/ui/checkbox'
import { Input } from '@/components/ui/input'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select'
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table'
import {
  ApiError,
  containersApi,
  frameworksApi,
  locationsApi,
  shiftTypesApi,
  type FrameworkResponse,
  type LocationTypeResponse,
  type ShiftTypeResponse,
} from '@/api'
import { useSeasonStore } from '@/stores/season'
import { useLocationsStore } from '@/stores/locations'
import { useErrors } from '@/composables/useErrors'
import {
  addDays,
  dayKey,
  formatDayShort,
  instantAt,
  minutesToClock,
  minutesToDuration,
} from '@/domain/time'
import { cn } from '@/lib/utils'

const { t } = useI18n()
const season = useSeasonStore()
const locationsStore = useLocationsStore()
const { report, messageOf } = useErrors()

const STORAGE_KEY = 'setup-wizard-draft'
const steps = computed(() => [
  t('wizard_step_types'),
  t('wizard_step_framework'),
  t('wizard_step_location'),
  t('wizard_step_containers'),
  t('wizard_step_summary'),
])
const step = ref(0)

interface NewType {
  name: string
  color: string
  offset: number
  adminOnly: boolean
  description: string
  createdId?: string
}
interface Draft {
  selectedTypeIds: string[]
  newTypes: NewType[]
  counts: Record<string, number>
  existingFrameworkId: string
  frameworkName: string
  frameworkMinutes: number
  createdFrameworkId?: string
  locationName: string
  locationTypeId: string
  createdLocationId?: string
  days: string[]
  startMinutes: number
  totalShifts: number
  color: string
  createdContainerDays: string[]
}
const draft = ref<Draft>(emptyDraft())
function emptyDraft(): Draft {
  return {
    selectedTypeIds: [],
    newTypes: [],
    counts: {},
    existingFrameworkId: '',
    frameworkName: '',
    frameworkMinutes: 90,
    locationName: '',
    locationTypeId: '',
    days: [],
    startMinutes: 8 * 60,
    totalShifts: 4,
    color: 'rgba(59, 130, 246, 0.30)',
    createdContainerDays: [],
  }
}

const existingTypes = ref<ShiftTypeResponse[]>([])
const existingFrameworks = ref<FrameworkResponse[]>([])
const locationTypes = ref<LocationTypeResponse[]>([])

onMounted(async () => {
  try {
    const raw = sessionStorage.getItem(STORAGE_KEY)
    if (raw) draft.value = { ...emptyDraft(), ...(JSON.parse(raw) as Draft) }
  } catch {
    /* ignore */
  }
  await loadLists()
})
watch(draft, (d) => sessionStorage.setItem(STORAGE_KEY, JSON.stringify(d)), { deep: true })

async function loadLists(): Promise<void> {
  if (!season.currentId) return
  try {
    ;[existingTypes.value, existingFrameworks.value, locationTypes.value] = await Promise.all([
      shiftTypesApi.list(season.currentId),
      frameworksApi.list(season.currentId),
      locationsApi.types(),
    ])
    if (!draft.value.locationTypeId && locationTypes.value[0])
      draft.value.locationTypeId = locationTypes.value[0].id
  } catch (error) {
    await report(error)
  }
}

/* step 1: types */
function toggleType(id: string, on: boolean): void {
  const set = new Set(draft.value.selectedTypeIds)
  if (on) set.add(id)
  else set.delete(id)
  draft.value.selectedTypeIds = [...set]
}
function addNewType(): void {
  draft.value.newTypes.push({
    name: '',
    color: '#e11d48',
    offset: 0,
    adminOnly: false,
    description: '',
  })
}
const chosenTypes = computed(() => [
  ...existingTypes.value
    .filter((x) => draft.value.selectedTypeIds.includes(x.id))
    .map((x) => ({ key: x.id, name: x.name, color: x.color })),
  ...draft.value.newTypes
    .filter((x) => x.name.trim())
    .map((x, i) => ({ key: `new-${i}`, name: x.name, color: x.color })),
])

/* step 2: framework */
const useExistingFramework = computed(() => !!draft.value.existingFrameworkId)
const slotCapacity = computed(() => {
  if (useExistingFramework.value) {
    const f = existingFrameworks.value.find((x) => x.id === draft.value.existingFrameworkId)
    return f ? f.shiftTypeCounts.reduce((s, c) => s + c.count, 0) : 0
  }
  return chosenTypes.value.reduce((s, x) => s + (draft.value.counts[x.key] ?? 1), 0)
})

/* step 4: days */
const seasonDays = computed(() => {
  if (!season.current) return []
  const start = dayKey(season.current.startDate)
  const end = dayKey(season.current.endDate)
  const out: string[] = []
  for (let d = start, i = 0; d <= end && i < 120; d = addDays(d, 1), i++) out.push(d)
  return out
})
function toggleDay(d: string): void {
  const set = new Set(draft.value.days)
  if (set.has(d)) set.delete(d)
  else set.add(d)
  draft.value.days = [...set].sort()
}
const frameworkMinutes = computed(() => {
  if (useExistingFramework.value) {
    const f = existingFrameworks.value.find((x) => x.id === draft.value.existingFrameworkId)
    return f ? f.secondsPerShift / 60 : 0
  }
  return draft.value.frameworkMinutes
})
const preview = computed(() =>
  draft.value.days.map((d) => ({
    day: d,
    start: minutesToClock(draft.value.startMinutes),
    end: minutesToClock(
      draft.value.startMinutes + frameworkMinutes.value * draft.value.totalShifts,
    ),
    done: draft.value.createdContainerDays.includes(d),
  })),
)
const totalCapacity = computed(
  () => draft.value.days.length * draft.value.totalShifts * slotCapacity.value,
)

/* validation per step */
const stepError = computed(() => {
  const d = draft.value
  switch (step.value) {
    case 0:
      return chosenTypes.value.length ? '' : t('admin_framework_counts_required')
    case 1:
      return useExistingFramework.value || d.frameworkName.trim() ? '' : t('field_required')
    case 2:
      return d.locationName.trim() && d.locationTypeId ? '' : t('field_required')
    case 3:
      return d.days.length ? '' : t('wizard_days_hint')
    default:
      return ''
  }
})

/* run */
const running = ref(false)
const progress = ref('')
const failure = ref('')
const done = ref(false)

async function runAll(): Promise<void> {
  if (!season.currentId) return
  running.value = true
  failure.value = ''
  const d = draft.value
  const seasonId = season.currentId
  try {
    progress.value = t('wizard_progress_types')
    for (const nt of d.newTypes) {
      if (!nt.name.trim() || nt.createdId) continue
      const created = await shiftTypesApi.create({
        name: nt.name.trim(),
        color: nt.color,
        startingTimeShift: minutesToDuration(nt.offset),
        onlyAssignableByAdmin: nt.adminOnly,
        seasonId,
        description: nt.description.trim() || null,
      })
      nt.createdId = created.id
    }

    progress.value = t('wizard_progress_framework')
    let frameworkId = d.existingFrameworkId || d.createdFrameworkId
    if (!frameworkId) {
      const typeCounts = [
        ...d.selectedTypeIds.map((id) => ({ shiftTypeId: id, count: d.counts[id] ?? 1 })),
        ...d.newTypes
          .filter((x) => x.createdId)
          .map((x, i) => ({
            shiftTypeId: x.createdId!,
            count: d.counts[`new-${d.newTypes.indexOf(x)}`] ?? d.counts[`new-${i}`] ?? 1,
          })),
      ]
      const fw = await frameworksApi.create({
        name: d.frameworkName.trim(),
        secondsPerShift: d.frameworkMinutes * 60,
        seasonId,
        typeCounts,
      })
      frameworkId = fw.id
      d.createdFrameworkId = fw.id
    }

    progress.value = t('wizard_progress_location')
    let locationId = d.createdLocationId
    if (!locationId) {
      const loc = await locationsApi.create({
        name: d.locationName.trim(),
        typeId: d.locationTypeId,
        seasonId,
      })
      locationId = loc.id
      d.createdLocationId = loc.id
    }

    const pending = d.days.filter((x) => !d.createdContainerDays.includes(x))
    for (const [i, day] of pending.entries()) {
      progress.value = t('wizard_progress_containers', { done: i + 1, total: pending.length })
      await containersApi.create({
        frameworkId,
        locationId,
        start: instantAt(day, d.startMinutes).toISOString(),
        totalShifts: d.totalShifts,
        color: d.color,
      })
      d.createdContainerDays.push(day)
    }

    done.value = true
    toast.success(t('wizard_done'))
    if (season.currentId) await locationsStore.load(season.currentId).catch(() => undefined)
  } catch (error) {
    failure.value =
      t('wizard_failed', { step: progress.value }) +
      ' ' +
      (error instanceof ApiError ? messageOf(error) : '')
    if (!(error instanceof ApiError)) await report(error)
  } finally {
    running.value = false
  }
}

function reset(): void {
  draft.value = emptyDraft()
  sessionStorage.removeItem(STORAGE_KEY)
  done.value = false
  failure.value = ''
  step.value = 0
  if (locationTypes.value[0]) draft.value.locationTypeId = locationTypes.value[0].id
}
</script>

<template>
  <div class="mx-auto max-w-3xl">
    <PageHeader :title="t('wizard_title')" :subtitle="t('wizard_subtitle')" />

    <ol class="mb-6 flex flex-wrap gap-2">
      <li v-for="(label, i) in steps" :key="label">
        <button
          type="button"
          :class="
            cn(
              'rounded-full border px-3 py-1 text-xs font-semibold',
              i === step
                ? 'border-primary bg-primary text-primary-foreground'
                : i < step
                  ? 'border-primary/50 text-primary'
                  : 'border-border text-muted-foreground',
            )
          "
          :disabled="i > step && !!stepError"
          @click="i <= step && (step = i)"
        >
          {{ i + 1 }}. {{ label }}
        </button>
      </li>
    </ol>

    <div class="space-y-5 rounded-xl border border-border bg-card p-4">
      <!-- 1 types -->
      <template v-if="step === 0">
        <h2 class="font-semibold">{{ t('wizard_types_existing') }}</h2>
        <div class="flex flex-wrap gap-2">
          <label
            v-for="x in existingTypes"
            :key="x.id"
            class="flex items-center gap-2 rounded-lg border border-border px-3 py-1.5 text-sm"
          >
            <Checkbox
              :model-value="draft.selectedTypeIds.includes(x.id)"
              @update:model-value="toggleType(x.id, $event === true)"
            />
            <span class="inline-block size-3 rounded-full" :style="{ background: x.color }" />{{
              x.name
            }}
          </label>
          <p v-if="!existingTypes.length" class="text-sm text-muted-foreground">–</p>
        </div>
        <h2 class="font-semibold">{{ t('wizard_types_new') }}</h2>
        <div
          v-for="(nt, i) in draft.newTypes"
          :key="i"
          class="grid gap-3 rounded-lg border border-border p-3 md:grid-cols-2"
        >
          <FormField :label="t('admin_shift_type_name')"><Input v-model="nt.name" /></FormField>
          <FormField :label="t('admin_shift_type_color')"
            ><ColorField v-model="nt.color"
          /></FormField>
          <FormField :label="t('admin_shift_type_offset')"
            ><Input
              v-model.number="nt.offset"
              type="number"
              min="-180"
              max="180"
              step="5"
              class="w-32"
          /></FormField>
          <label class="flex items-center gap-2 self-end pb-2 text-sm"
            ><Checkbox
              :model-value="nt.adminOnly"
              @update:model-value="nt.adminOnly = $event === true"
            />{{ t('admin_shift_type_admin_only') }}</label
          >
          <FormField class="md:col-span-2" :label="t('admin_shift_type_description')"
            ><Input v-model="nt.description"
          /></FormField>
          <div class="md:col-span-2 flex justify-end">
            <Button
              type="button"
              variant="ghost"
              size="sm"
              :disabled="!!nt.createdId"
              @click="draft.newTypes.splice(i, 1)"
              ><X /> {{ t('btn_delete') }}</Button
            >
          </div>
        </div>
        <Button type="button" variant="outline" size="sm" @click="addNewType"
          ><Plus /> {{ t('btn_new') }}</Button
        >
      </template>

      <!-- 2 framework -->
      <template v-else-if="step === 1">
        <FormField :label="t('wizard_framework_existing')">
          <Select
            :model-value="draft.existingFrameworkId || '__new__'"
            @update:model-value="
              draft.existingFrameworkId = $event === '__new__' ? '' : String($event)
            "
          >
            <SelectTrigger class="w-full"><SelectValue /></SelectTrigger>
            <SelectContent>
              <SelectItem value="__new__">{{ t('wizard_framework_new') }}</SelectItem>
              <SelectItem v-for="f in existingFrameworks" :key="f.id" :value="f.id">{{
                f.name
              }}</SelectItem>
            </SelectContent>
          </Select>
        </FormField>
        <template v-if="!useExistingFramework">
          <FormField :label="t('admin_framework_name')"
            ><Input v-model="draft.frameworkName"
          /></FormField>
          <FormField :label="t('admin_framework_duration')"
            ><DurationField v-model="draft.frameworkMinutes" :step="5" :min="5"
          /></FormField>
          <FormField :label="t('admin_framework_counts')">
            <div class="space-y-2">
              <div v-for="x in chosenTypes" :key="x.key" class="flex items-center gap-3">
                <span class="inline-block size-3 rounded-full" :style="{ background: x.color }" />
                <span class="flex-1 text-sm">{{ x.name }}</span>
                <Input
                  :model-value="draft.counts[x.key] ?? 1"
                  type="number"
                  min="1"
                  max="99"
                  class="w-20"
                  @update:model-value="draft.counts[x.key] = Number($event)"
                />
              </div>
            </div>
          </FormField>
        </template>
      </template>

      <!-- 3 location -->
      <template v-else-if="step === 2">
        <FormField :label="t('admin_location_name')"
          ><Input v-model="draft.locationName"
        /></FormField>
        <FormField :label="t('admin_location_type')">
          <Select v-model="draft.locationTypeId">
            <SelectTrigger class="w-full"><SelectValue /></SelectTrigger>
            <SelectContent>
              <SelectItem v-for="x in locationTypes" :key="x.id" :value="x.id">{{
                x.name
              }}</SelectItem>
            </SelectContent>
          </Select>
        </FormField>
      </template>

      <!-- 4 containers -->
      <template v-else-if="step === 3">
        <FormField :label="t('wizard_days')" :hint="t('wizard_days_hint')">
          <div class="flex flex-wrap gap-1.5">
            <button
              v-for="d in seasonDays"
              :key="d"
              type="button"
              :class="
                cn(
                  'rounded-lg border px-2 py-1 text-xs',
                  draft.days.includes(d)
                    ? 'border-primary bg-primary text-primary-foreground'
                    : 'border-border text-muted-foreground hover:bg-accent',
                )
              "
              @click="toggleDay(d)"
            >
              {{ formatDayShort(instantAt(d, 720)) }}
            </button>
          </div>
        </FormField>
        <div class="grid gap-3 md:grid-cols-3">
          <FormField :label="t('shift_start')"
            ><Input
              type="time"
              step="900"
              :model-value="minutesToClock(draft.startMinutes)"
              @update:model-value="
                draft.startMinutes =
                  Number(String($event).split(':')[0]) * 60 +
                  Number(String($event).split(':')[1] ?? 0)
              "
          /></FormField>
          <FormField :label="t('admin_container_count')"
            ><Input v-model.number="draft.totalShifts" type="number" min="1" max="48"
          /></FormField>
          <FormField :label="t('admin_container_color')"
            ><ColorField v-model="draft.color" alpha
          /></FormField>
        </div>
        <h3 class="font-semibold">{{ t('wizard_preview') }}</h3>
        <Table>
          <TableHeader
            ><TableRow
              ><TableHead>{{ t('wizard_days') }}</TableHead
              ><TableHead>{{ t('shift_start') }}</TableHead
              ><TableHead>{{ t('shift_end') }}</TableHead
              ><TableHead>{{ t('stat_shifts') }}</TableHead></TableRow
            ></TableHeader
          >
          <TableBody>
            <TableRow v-for="p in preview" :key="p.day"
              ><TableCell>{{ formatDayShort(instantAt(p.day, 720)) }}</TableCell
              ><TableCell>{{ p.start }}</TableCell
              ><TableCell>{{ p.end }}</TableCell
              ><TableCell>{{ draft.totalShifts * slotCapacity }}</TableCell></TableRow
            >
          </TableBody>
        </Table>
      </template>

      <!-- 5 summary -->
      <template v-else>
        <dl class="grid gap-2 text-sm md:grid-cols-[10rem_1fr]">
          <dt class="text-muted-foreground">{{ t('wizard_step_types') }}</dt>
          <dd>{{ chosenTypes.map((x) => x.name).join(', ') }}</dd>
          <dt class="text-muted-foreground">{{ t('wizard_step_framework') }}</dt>
          <dd>
            {{
              useExistingFramework
                ? existingFrameworks.find((f) => f.id === draft.existingFrameworkId)?.name
                : `${draft.frameworkName} (${minutesToClock(draft.frameworkMinutes)} h)`
            }}
          </dd>
          <dt class="text-muted-foreground">{{ t('wizard_step_location') }}</dt>
          <dd>
            {{ draft.locationName }} ·
            {{ locationTypes.find((x) => x.id === draft.locationTypeId)?.name }}
          </dd>
          <dt class="text-muted-foreground">{{ t('wizard_step_containers') }}</dt>
          <dd>
            {{ draft.days.length }} × {{ minutesToClock(draft.startMinutes) }},
            {{ draft.totalShifts }} {{ t('stat_shifts') }}
          </dd>
        </dl>
        <p class="font-semibold">{{ t('wizard_capacity', { count: totalCapacity }) }}</p>
        <p v-if="running" class="text-sm text-muted-foreground">{{ progress }} …</p>
        <p
          v-if="failure"
          class="rounded-lg border border-destructive/50 bg-destructive/10 p-3 text-sm"
        >
          {{ failure }}
        </p>
        <div
          v-if="done"
          class="space-y-2 rounded-lg border border-status-green/50 bg-status-green/10 p-3 text-sm"
        >
          <p class="flex items-center gap-2"><Check class="size-4" /> {{ t('wizard_done') }}</p>
          <div class="flex gap-2">
            <Button size="sm" as-child
              ><RouterLink :to="`/locations/${draft.createdLocationId}`">{{
                t('wizard_open_location')
              }}</RouterLink></Button
            >
            <Button size="sm" variant="outline" as-child
              ><RouterLink :to="`/admin/locations/${draft.createdLocationId}/containers`">{{
                t('wizard_open_admin')
              }}</RouterLink></Button
            >
            <Button size="sm" variant="ghost" @click="reset">{{ t('btn_new') }}</Button>
          </div>
        </div>
      </template>

      <p v-if="stepError && step < 4" class="text-xs text-muted-foreground">{{ stepError }}</p>
      <div class="flex justify-between gap-2 pt-2">
        <Button variant="ghost" :disabled="step === 0 || running" @click="step--">{{
          t('btn_back')
        }}</Button>
        <Button v-if="step < 4" :disabled="!!stepError" @click="step++">{{ t('btn_next') }}</Button>
        <Button v-else :disabled="running || done" @click="runAll">{{
          failure ? t('btn_retry') : t('wizard_run')
        }}</Button>
      </div>
    </div>
  </div>
</template>
